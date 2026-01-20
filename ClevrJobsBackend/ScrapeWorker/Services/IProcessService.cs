using Data.Models;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Services
{
    public interface IProcessService
    {
        Task<bool> ProcessRawJobs(ICollection<RawJob> rawJobs, IProcessRepository processRepository);
    }
}
