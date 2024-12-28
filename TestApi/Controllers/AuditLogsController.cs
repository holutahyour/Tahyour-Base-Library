using Tahyour.Base.Common.Domain.Entities;
using Tahyour.Base.Common.Presentation;
using Tahyour.Base.Common.Services.Interface;

namespace TestApi.Controllers
{

    public class AuditLogsController : MongoBaseController<AuditLog, AuditLog>
    {
        public AuditLogsController(IMongoBaseService<AuditLog> service) : base(service) { }

    }

}
