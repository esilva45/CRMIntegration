using CRMIntegration.Model;
using Npgsql;
using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace CRMIntegration.Utils {
    class SendMessage {

        public static void Message() {
            CallModel call = new CallModel();
            string call_id = "";
            string result = "";

            try {
                string query = "select myphone_callhistory_v14.call_id, dnowner, " +
                    "party_dn, to_char(start_time, 'YYYY-MM-DD HH24:MI') as start_time, " +
                    "to_char(end_time, 'YYYY-MM-DD HH24:MI') as end_time, " +
                    "(date_part('second', end_time - start_time) + (date_part('minute', end_time - start_time) * 60)) as tempo, " +
                    "to_char(established_time, 'YYYYMMDDHH24MISS') as established " +
                    "from myphone_callhistory_v14 " +
                    "inner join crm_integration on myphone_callhistory_v14.call_id = crm_integration.call_id " +
                    "where crm_integration.processed = false";

                NpgsqlConnection conn = Connection.GetConnection();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader rd = command.ExecuteReader();

                while (rd.Read()) {
                    result = "";
                    call.UrlLigacao = (string)Properties.Settings.Default["UrlLigacao"] + Util.FindDirectory(rd["dnowner"].ToString(), rd["party_dn"].ToString(), rd["established"].ToString());
                    call.OrigemTel = rd["dnowner"].ToString();
                    call.DestinoTel = rd["party_dn"].ToString();
                    call.DtInicioChamada = rd["start_time"].ToString();
                    call.DtFimChamada = rd["end_time"].ToString();
                    call.TempoConversacao = rd["tempo"].ToString();
                    result = Send(JsonConvert.SerializeObject(call), rd["call_id"].ToString());

                    if (result.Equals("OK")) {
                        call_id += rd["call_id"] + " ";
                    }
                }

                if (call_id != "") {
                    call_id = call_id.Trim().Replace(" ", ",");
                    NpgsqlCommand cmd = new NpgsqlCommand("update crm_integration set processed = true where call_id in (" + call_id + ")", conn);
                    cmd.ExecuteReader();
                }
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }
            finally {
                Connection.CloseConnection();
            }
        }

        private static string Send(string json, string call_id) {
            string code = "ERROR";

            try {
                string url_service = (string)Properties.Settings.Default["UrlService"];
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url_service);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers["Token_Exact"] = "78AE56DB-A6C5-4CD3-BAF6-D6A1B64223EB";
                httpWebRequest.Method = "POST";

                byte[] data = Encoding.UTF8.GetBytes(json);
                httpWebRequest.ContentLength = data.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                HttpStatusCode respStatusCode = httpResponse.StatusCode;

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    HttpStatusCode statusCode = ((HttpWebResponse)httpResponse).StatusCode;
                    var result = streamReader.ReadToEnd();
                    code = statusCode.ToString();
                    Util.Log("ID: " + call_id + " code: " + code);
                    Util.Log(json);
                }
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }

            return code;
        }
    }
}
