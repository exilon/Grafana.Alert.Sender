using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IAlertService
    {
        public Task Handle(string contactName, GrafanaAlert grafanaAlert, CancellationToken cancellationToken = default(CancellationToken));
    }
}
