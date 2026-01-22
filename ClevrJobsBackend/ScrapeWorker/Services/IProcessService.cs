using Data.Models;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Models.Dto.Responses;

namespace Workers.Services
{
    public interface IProcessService
    {
        Task<ServiceResponse> ProcessRawJobs(ICollection<RawJob> rawJobs, IProcessRepository processRepository);
    }
}
