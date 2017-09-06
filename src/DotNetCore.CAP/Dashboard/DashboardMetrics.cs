﻿using System;
using System.Collections.Generic;
using System.Linq;
using DotNetCore.CAP.Dashboard.Resources;

namespace DotNetCore.CAP.Dashboard
{
    public static class DashboardMetrics
    {
        private static readonly Dictionary<string, DashboardMetric> Metrics = new Dictionary<string, DashboardMetric>();

        static DashboardMetrics()
        {
            AddMetric(ServerCount);
            AddMetric(RetriesCount);

            AddMetric(PublishedFailedCountOrNull);
            AddMetric(ReceivedFailedCountOrNull);

            AddMetric(PublishedProcessingCount);
            AddMetric(ReceivedProcessingCount);

            AddMetric(PublishedSucceededCount);
            AddMetric(ReceivedSucceededCount);

            AddMetric(PublishedFailedCount);
            AddMetric(ReceivedFailedCount);
        }

        public static void AddMetric(DashboardMetric metric)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));

            lock (Metrics)
            {
                Metrics[metric.Name] = metric;
            }
        }

        public static IEnumerable<DashboardMetric> GetMetrics()
        {
            lock (Metrics)
            {
                return Metrics.Values.ToList();
            }
        }

        public static readonly DashboardMetric ServerCount = new DashboardMetric(
            "servers:count",
            "Metrics_Servers",
            page => new Metric(page.Statistics.Servers.ToString("N0"))
            {
                Style = page.Statistics.Servers == 0 ? MetricStyle.Warning : MetricStyle.Default,
                Highlighted = page.Statistics.Servers == 0,
                Title = page.Statistics.Servers == 0
                    ? "No active servers found. Jobs will not be processed."
                    : null
            });

        public static readonly DashboardMetric RetriesCount = new DashboardMetric(
            "retries:count",
             "Metrics_Retries",
            page =>
            {
                long retryCount;
                using (var connection = page.Storage.GetConnection())
                {
                    var storageConnection = connection as IStorageConnection;
                    if (storageConnection == null)
                    {
                        return null;
                    }

                    retryCount = storageConnection.GetSetCount("retries");
                }

                return new Metric(retryCount.ToString("N0"))
                {
                    Style = retryCount > 0 ? MetricStyle.Warning : MetricStyle.Default
                };
            });

        //----------------------------------------------------

        public static readonly DashboardMetric PublishedFailedCountOrNull = new DashboardMetric(
            "published_failed:count-or-null",
            "Metrics_FailedJobs",
            page => page.Statistics.PublishedFailed > 0
                ? new Metric(page.Statistics.PublishedFailed.ToString("N0"))
                {
                    Style = MetricStyle.Danger,
                    Highlighted = true,
                    Title = string.Format(Strings.Metrics_FailedCountOrNull, page.Statistics.PublishedFailed)
                }
                : null);

        public static readonly DashboardMetric ReceivedFailedCountOrNull = new DashboardMetric(
            "received_failed:count-or-null",
            "Metrics_FailedJobs",
             page => page.Statistics.ReceivedFailed > 0
             ? new Metric(page.Statistics.ReceivedFailed.ToString("N0"))
             {
                 Style = MetricStyle.Danger,
                 Highlighted = true,
                 Title = string.Format(Strings.Metrics_FailedCountOrNull, page.Statistics.ReceivedFailed)
             }
             : null);

        //----------------------------------------------------

        public static readonly DashboardMetric PublishedProcessingCount = new DashboardMetric(
            "published_processing:count",
            "Metrics_ProcessingJobs",
            page => new Metric(page.Statistics.PublishedProcessing.ToString("N0"))
            {
                Style = page.Statistics.PublishedProcessing > 0 ? MetricStyle.Warning : MetricStyle.Default
            });

        public static readonly DashboardMetric ReceivedProcessingCount = new DashboardMetric(
           "received_processing:count",
           "Metrics_ProcessingJobs",
           page => new Metric(page.Statistics.ReceivedProcessing.ToString("N0"))
           {
               Style = page.Statistics.ReceivedProcessing > 0 ? MetricStyle.Warning : MetricStyle.Default
           });

        //----------------------------------------------------
        public static readonly DashboardMetric PublishedSucceededCount = new DashboardMetric(
            "published_succeeded:count",
            "Metrics_SucceededJobs",
            page => new Metric(page.Statistics.PublishedSucceeded.ToString("N0"))
            {
                IntValue = page.Statistics.PublishedSucceeded
            });

        public static readonly DashboardMetric ReceivedSucceededCount = new DashboardMetric(
           "received_succeeded:count",
           "Metrics_SucceededJobs",
           page => new Metric(page.Statistics.ReceivedSucceeded.ToString("N0"))
           {
               IntValue = page.Statistics.ReceivedSucceeded
           });


        //----------------------------------------------------

        public static readonly DashboardMetric PublishedFailedCount = new DashboardMetric(
            "published_failed:count",
            "Metrics_FailedJobs",
            page => new Metric(page.Statistics.PublishedFailed.ToString("N0"))
            {
                IntValue = page.Statistics.PublishedFailed,
                Style = page.Statistics.PublishedFailed > 0 ? MetricStyle.Danger : MetricStyle.Default,
                Highlighted = page.Statistics.PublishedFailed > 0
            });
        public static readonly DashboardMetric ReceivedFailedCount = new DashboardMetric(
           "received_failed:count",
           "Metrics_FailedJobs",
           page => new Metric(page.Statistics.ReceivedFailed.ToString("N0"))
           {
               IntValue = page.Statistics.ReceivedFailed,
               Style = page.Statistics.ReceivedFailed > 0 ? MetricStyle.Danger : MetricStyle.Default,
               Highlighted = page.Statistics.ReceivedFailed > 0
           });
    }
}
