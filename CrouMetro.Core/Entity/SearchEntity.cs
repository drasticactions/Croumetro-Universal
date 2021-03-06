﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class SearchEntity
    {
        public decimal CompletedIn { private set; get; }

        public long MaxId { private set; get; }

        public string MaxIdStr { private set; get; }

        public long SinceId { private set; get; }

        public string SinceIdStr { private set; get; }

        public int Count { private set; get; }

        public string NextResults { private set; get; }

        public string Query { private set; get; }

        public string RefreshUrl { private set; get; }

        public ObservableCollection<PostEntity> PostList { private set; get; }

        public static SearchEntity ParseStatuses(string statuses, string searchMetaData,
            UserAccountEntity userAccountEntity)
        {
            if (statuses == null) throw new ArgumentNullException("statuses");
            JArray statusList = JArray.Parse(statuses);
            JObject searchMeta = JObject.Parse(searchMetaData);
            var searchEntity = new SearchEntity
            {
                PostList = statusList != null ? PostEntity.Parse(statusList.ToString(), userAccountEntity) : null,
                CompletedIn = searchMeta["completed_in"] != null ? (Decimal) searchMeta["completed_in"] : 0,
                MaxId = searchMeta["max_id"] != null ? long.Parse((String) searchMeta["max_id"]) : 0,
                MaxIdStr = searchMeta["max_id_str"] != null ? (String) searchMeta["max_id_str"] : string.Empty,
                SinceId = searchMeta["since_id"] != null ? long.Parse((String) searchMeta["since_id"]) : 0,
                SinceIdStr = searchMeta["since_id_str"] != null ? (String) searchMeta["since_id_str"] : string.Empty,
                Count = searchMeta["count"] != null ? (int) searchMeta["count"] : 0,
                NextResults = searchMeta["next_results"] != null ? (String) searchMeta["next_results"] : string.Empty,
                RefreshUrl = searchMeta["refresh_url"] != null ? (String) searchMeta["refresh_url"] : string.Empty,
                Query = searchMeta["query"] != null ? (String) searchMeta["query"] : string.Empty
            };
            return searchEntity;
        }

        private static long FixTime(String time)
        {
            String[] times = time.Split(' ');
            String day = times[1];
            String mon = times[2];
            String year = times[3];
            String hour = times[4].Split(':')[0];
            String min = times[4].Split(':')[1];
            String sec = times[4].Split(':')[2];
            String utf = times[5];

            var date = new DateTime(int.Parse(year), FixMonth(mon), int.Parse(day), int.Parse(hour), int.Parse(min),
                int.Parse(sec), DateTimeKind.Local);
            return UserAccountEntity.GetUnixTime(date);
        }

        private static int FixMonth(String m)
        {
            switch (m)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return 1;
            }
        }

        private static String ToDate(long unix)
        {
            TimeZoneInfo ti = TimeZoneInfo.Local;
            DateTime time =
                TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix), ti);
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}