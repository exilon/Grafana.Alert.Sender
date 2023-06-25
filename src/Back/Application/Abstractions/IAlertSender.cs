using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAlertSender
    {
        public Task Send(string contactName, Alert alert, CancellationToken cancellationToken);
    }
}
