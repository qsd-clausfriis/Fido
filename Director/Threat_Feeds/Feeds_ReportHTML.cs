using System;
using System.IO;
using Fido_Main.Fido_Support.ErrorHandling;
using Fido_Main.Fido_Support.Objects.ThreatGRID;

namespace Fido_Main.Director.Threat_Feeds
{
    static class Feeds_ReportHTML
    {
        public static void ReportHTML(string sHash)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            var parseConfigs = Object_ThreatGRID_Configs.GetThreatGridConfigs("report-level");
            var request = parseConfigs.ApiBaseUrl + parseConfigs.ApiFuncCall + sHash + "/report.html?" + parseConfigs.ApiQueryString + "&api_key=" + parseConfigs.ApiKey;
            var alertRequest = (HttpWebRequest)WebRequest.Create(request);
            alertRequest.Method = "GET";
            try
            {
                using (var ThreatGRIDResponse = alertRequest.GetResponse() as HttpWebResponse)
                {
                    if (ThreatGRIDResponse != null && ThreatGRIDResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var respStream = ThreatGRIDResponse.GetResponseStream())
                        {
                            if (respStream == null) return;
                            //todo: move this to the DB
                            using (var file = File.Create(Environment.CurrentDirectory + @"\reports\threatgrid\" + sHash + ".html"))
                            {
                                respStream.CopyTo(file);
                            }
                            ThreatGRIDResponse.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Fido_EventHandler.SendEmail("Fido Error", "Fido Failed: {0} Exception caught downloading ThreatGRID report information:" + e);
            }
        }
    }
}