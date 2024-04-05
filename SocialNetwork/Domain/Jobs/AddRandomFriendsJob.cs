using Microsoft.Extensions.Configuration.UserSecrets;
using Quartz;
using Quartz.Impl;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Infrastructure.Data;

namespace SocialNetwork.Domain.Jobs
{
    public class AddRandomFriendsJob : IJob
    {
        private readonly SocialNetworkContext _context;
        public static readonly JobKey Key = new JobKey("AddRandomFriendsJob");

        public AddRandomFriendsJob(SocialNetworkContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var usersList = _context.Users.ToList();

            foreach (var user in usersList)
            {
                while (true)
                {
                    var randomUser = usersList[new Random().Next(usersList.Count())];

                    var usersRandomFriends = _context.RandomFriends.Where(randFriends => randFriends.User == user.Id).Select(randFriends => randFriends.RandomFriendOfUser).ToList();

                    if (randomUser is not null && user.Id != randomUser.Id)
                    {
                        if (!usersRandomFriends.Contains(randomUser.Id))
                        {
                            var newRandomFriend = new RandomFriend(user.Id.ToString(), randomUser.Id.ToString());

                            await _context.RandomFriends.AddAsync(newRandomFriend);
                            await _context.SaveChangesAsync();

                            break;
                        }
                        else
                        {
                            var randomFriendItem = _context.RandomFriends.FirstOrDefault(randFriend => randFriend.User == user.Id && randFriend.RandomFriendOfUser == randomUser.Id);
                            if (randomFriendItem.ExpirationTime <= ToDateOnly(DateTime.Now))
                            {
                                var newRandomFriend = new RandomFriend(user.Id.ToString(), randomUser.Id.ToString());

                                _context.RandomFriends.Update(newRandomFriend);

                                await _context.SaveChangesAsync();

                                break;
                            }
                        }
                    }

                }
            }
        }

        public static DateOnly ToDateOnly(DateTime dateTime)
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }

    public class TransactionScheduler
    {
        public static async Task Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<AddRandomFriendsJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
