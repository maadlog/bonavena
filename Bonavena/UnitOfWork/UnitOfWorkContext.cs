//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Bonavena.UnitOfWork
//{
//    public class UnitOfWorkContext : IUnitOfWork
//    {
//        private TransactionScope Active;
//        private Boolean Transactioning { get { return TranCount > 0; } }
//        private Int16 TranCount;
//        private bool Disabled;

//        public UnitOfWorkContext()
//        {
//            TranCount = 0;
//            var stringDis = ConfigurationManager.AppSettings["DisableUnitOfWork"];

//            Disabled = !String.IsNullOrEmpty(stringDis) && bool.Parse(stringDis);
//        }

//        public void Begin()
//        {
//            if (Disabled) return;
//            try
//            {
//                if (!Transactioning)
//                {
//                    Active = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
//                    {
//                        IsolationLevel = IsolationLevel.ReadUncommitted
//                    });
//                }

//                TranCount += 1;

//            }
//            catch
//            {
//                if (Active != null)
//                {
//                    Active.Dispose();
//                    Active = null;
//                }
//                throw;
//            }
//        }

//        public void Commit()
//        {
//            if (Disabled) return;
//            if (Transactioning)
//            {
//                if (TranCount == 1)
//                {
//                    try
//                    {
//                        Active.Complete();
//                    }
//                    catch
//                    {
//                        throw;
//                    }
//                    finally
//                    {
//                        Active.Dispose();
//                        Active = null;
//                    }
//                }
//                TranCount -= 1;
//            }
//            else
//            {
//                //Una vez rollbackeada la transacción (Transactioning == false)
//                // no se puede commitear.
//                throw new ClosedUnitOfWorkException("Transaction already closed");
//            }
//        }

//        public void Rollback()
//        {
//            if (Disabled) return;
//            if (Transactioning)
//            {
//                if (Active != null)
//                {
//                    Active.Dispose();
//                    Active = null;
//                    TranCount = 0;
//                }
//            }
//            else
//            {
//                //Una vez rollbackeada la transacción (Transactioning == false)
//                // el rollback no tiene efecto.
//            }
//        }
//    }
//}
