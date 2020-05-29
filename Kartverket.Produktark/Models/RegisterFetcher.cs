﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Produktark.Models
{
    public class RegisterFetcher
    {
        MemoryCacher memCacher = new MemoryCacher();

        Dictionary<string, string> TopicCategories = new Dictionary<string, string>();
        Dictionary<string, string> SpatialRepresentations = new Dictionary<string, string>();
        Dictionary<string, string> MaintenanceFrequencyValues = new Dictionary<string, string>();
        Dictionary<string, string> ListOfStatusValues = new Dictionary<string, string>();
        Dictionary<string, string> ListOfClassificationValues = new Dictionary<string, string>();
        Dictionary<string, string> ListOfRestrictionValues = new Dictionary<string, string>();
        Dictionary<string, string> ListOfCoordinatesystemNameValues = new Dictionary<string, string>();
        Dictionary<string, string> ListOfDistributionTypes = new Dictionary<string, string>();


        public RegisterFetcher()
        {
            TopicCategories = GetCodeList("9A46038D-16EE-4562-96D2-8F6304AAB100");
            SpatialRepresentations = GetCodeList("4C54EB31-714E-4457-AF6A-44FE6DBE76C1");
            MaintenanceFrequencyValues = GetCodeList("9A46038D-16EE-4562-96D2-8F6304AAB124");
            ListOfStatusValues = GetCodeList("9A46038D-16EE-4562-96D2-8F6304AAB137");
            ListOfClassificationValues = GetCodeList("9A46038D-16EE-4562-96D2-8F6304AAB145");
            ListOfRestrictionValues = GetCodeList("D23E9F2F-66AB-427D-8AE4-5B6FD3556B57");
            ListOfCoordinatesystemNameValues = GetEPSGCodeList("37B9DC41-D868-4CBC-84F9-39557041FB2C");
            ListOfDistributionTypes = GetCodeList("94B5A165-7176-4F43-B6EC-1063F7ADE9EA");

        }

        public string GetCoordinatesystemName(string value)
        {
            KeyValuePair<string, string> dic = ListOfCoordinatesystemNameValues.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }
        public string GetDistributionType(string value)
        {
            KeyValuePair<string, string> dic = ListOfDistributionTypes.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }

        public string GetSpatialRepresentation(string value)
        {
            KeyValuePair<string, string> dic = SpatialRepresentations.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }

        public string GetTopicCategory(string value)
        {
            KeyValuePair<string, string> dic = TopicCategories.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }

        public string GetMaintenanceFrequency(string value)
        {
            KeyValuePair<string, string> dic = MaintenanceFrequencyValues.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }

        public string GetStatus(string value)
        {
            KeyValuePair<string, string> dic = ListOfStatusValues.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }

        public string GetClassification(string value)
        {
            KeyValuePair<string, string> dic = ListOfClassificationValues.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            return value;
        }

        public string GetRestriction(string value, string OtherConstraintsAccess = "")
        {
            KeyValuePair<string, string> dic = ListOfRestrictionValues.Where(p => p.Key == value).FirstOrDefault();
            if (!dic.Equals(default(KeyValuePair<String, String>)))
                value = dic.Value;

            Dictionary<string, string> inspire = GetInspireAccessRestrictions();

            if (value == "restricted")
                value = inspire["https://inspire.ec.europa.eu/metadata-codelist/LimitationsOnPublicAccess/INSPIRE_Directive_Article13_1b"];
            if (value == "no restrictions" || OtherConstraintsAccess == "no restrictions")
                value = inspire["https://inspire.ec.europa.eu/metadata-codelist/LimitationsOnPublicAccess/noLimitations"];
            else if (value == "norway digital restricted" || OtherConstraintsAccess == "norway digital restricted")
                value = inspire["https://inspire.ec.europa.eu/metadata-codelist/LimitationsOnPublicAccess/INSPIRE_Directive_Article13_1d"];

            return value;
        }



        public Dictionary<string, string> GetCodeList(string systemid)
        {
            var cache = memCacher.GetValue(systemid);

            Dictionary<string, string> CodeValues = new Dictionary<string, string>();

            if (cache != null)
            {
                CodeValues = cache as Dictionary<string, string>;
            }
            else
            {

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "api/kodelister/" + systemid;
                System.Net.WebClient c = new System.Net.WebClient();
                c.Encoding = System.Text.Encoding.UTF8;
                var data = c.DownloadString(url);
                var response = Newtonsoft.Json.Linq.JObject.Parse(data);

                var codeList = response["containeditems"];

                foreach (var code in codeList)
                {
                    var codevalue = code["codevalue"].ToString();
                    if (string.IsNullOrWhiteSpace(codevalue))
                        codevalue = code["label"].ToString();

                    if (!CodeValues.ContainsKey(codevalue))
                    {
                        CodeValues.Add(codevalue, code["label"].ToString());
                    }
                }

                CodeValues = CodeValues.OrderBy(o => o.Value).ToDictionary(o => o.Key, o => o.Value);

                memCacher.Add(systemid, CodeValues, new DateTimeOffset(DateTime.Now.AddHours(12)));

            }

            return CodeValues;
        }

        public Dictionary<string, string> GetEPSGCodeList(string systemid)
        {
            var cache = memCacher.GetValue(systemid);

            Dictionary<string, string> CodeValues = new Dictionary<string, string>();

            if (cache != null)
            {
                CodeValues = cache as Dictionary<string, string>;
            }
            else
            {

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "api/kodelister/" + systemid;
                System.Net.WebClient c = new System.Net.WebClient();
                c.Encoding = System.Text.Encoding.UTF8;
                var data = c.DownloadString(url);
                var response = Newtonsoft.Json.Linq.JObject.Parse(data);

                var codeList = response["containeditems"];

                foreach (var code in codeList)
                {
                    var codevalue = code["documentreference"].ToString();
                    if (string.IsNullOrWhiteSpace(codevalue))
                        codevalue = code["label"].ToString();

                    if (!CodeValues.ContainsKey(codevalue))
                    {
                        CodeValues.Add(codevalue, code["label"].ToString());
                    }
                }

                CodeValues = CodeValues.OrderBy(o => o.Value).ToDictionary(o => o.Key, o => o.Value);

                memCacher.Add(systemid, CodeValues, new DateTimeOffset(DateTime.Now.AddHours(12)));

            }

            return CodeValues;
        }

        public Dictionary<string, string> GetInspireAccessRestrictions(string culture = "no")
        {
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            c.Headers.Remove("Accept-Language");
            c.Headers.Add("Accept-Language", culture);
            var data = c.DownloadString(System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "api/metadata-kodelister/inspire-tilgangsrestriksjoner");
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            Dictionary<string, string> inspire = new Dictionary<string, string>();

            var items = response["containeditems"];

            foreach (var item in items)
            {
                var id = item["codevalue"].ToString();
                string label = item["label"].ToString();
                string status = item["status"].ToString();



                if (status == "Gyldig" || status == "Valid")
                {
                    inspire.Add(id, label);
                }
            }

            return inspire;
        }

    }
}