﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Maruko.Core.Application;
using Maruko.Core.FreeSql.Internal.Repos;
using Maruko.TaskScheduling.Internal.Strategy;
using Quartz;

namespace Maruko.TaskScheduling
{
    public class StrategyBase<TJob> : IStrategy
        where TJob : IJob
    {
        private readonly ISchedulerFactory _factory;
        private readonly IFreeSqlRepository<TaskScheduling> _taskSchedule;

        protected StrategyBase(ISchedulerFactory factory, IFreeSqlRepository<TaskScheduling> taskSchedule)
        {
            _factory = factory;
            _taskSchedule = taskSchedule;
        }

        protected async Task<IScheduler> GetSchedulerAsync()
        {
            return await _factory.GetScheduler();
        }

        public async Task<AjaxResponse<object>> ExecuteAsync(ExecuteRequest request)
        {
            var task = _taskSchedule.FirstOrDefault(request.ObjectId);

            if (task == null)
                return new AjaxResponse<object>("任务不存在");

            var scheduler = await GetSchedulerAsync();
            await scheduler.Start();

            var job = JobBuilder.Create<TJob>()
                .WithIdentity($"job_{request.ObjectId}", task.GroupName)
                .UsingJobData(Map(request.ObjectId))
                .Build();

            //创建一个触发器
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity($"trigger_{request.ObjectId}", task.GroupName)
                .WithCronSchedule(task.CronExpression)
                .ForJob(job);

            await scheduler.ScheduleJob(job, triggerBuilder.Build());

            task.StartTime = DateTime.Now;
            _taskSchedule.Update(task);
            return new AjaxResponse<object>("任务执行成功");
        }

        protected virtual JobDataMap Map(long objectId)
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>()
            {
                {"objectId",objectId}
            };
            return new JobDataMap(dictionary);
        }
    }
}