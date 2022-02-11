using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Services
{
    public interface FingerPrintService
    {
        string GetType();
        Task UserID();
        bool fingerprintEnabled();
    }
}
