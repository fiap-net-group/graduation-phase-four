using Graduation.Notification.Application.Email.Send.Boundaries;
using Mapster;
using MapsterMapper;
using System.Reflection;

namespace Graduation.Notification.UnitTests.Fixtures
{
    public static class MapperFixture
    {
        public static void AddMapper()
        {
            SendEmailMapper.Add();

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
