using System;
using System.Collections.Generic;
using System.Text;

namespace Bonavena.UnitOfWork
{
    public interface IUnitOfWork
    {
        void Begin();

        void Commit();

        void Rollback();
    }
}
