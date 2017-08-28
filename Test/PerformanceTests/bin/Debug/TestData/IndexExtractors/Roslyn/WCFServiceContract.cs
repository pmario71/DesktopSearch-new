/*[ Compilation unit ----------------------------------------------------------

-----------------------------------------------------------------------------*/
/*] END */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Tests.SomeNameSpace
{

    /// <summary>
    /// 
    /// </summary> 
    [ContractClass(typeof(ACodeContractContract))]
    [ServiceKnownType(typeof(SomeDataContract))]
    [ServiceContract(Name = "IWCFServiceInterface", Namespace = "Tests.SomeNameSpace",
                     SessionMode = SessionMode.Required,
                     CallbackContract = typeof(IWCFServiceInterfaceEvents))]
    [Export]
    
    public interface IWCFServiceInterface
    {
        [OperationContract(Name = "CreateProcedureForServiceWithDefault")]
        [FaultContract(typeof(AdapterFaultReport))]
        [TransactionFlow(TransactionFlowOption.Allowed)] //but useless since no DB access shall be performed under this method
        Result CreateOperation(InputParameter parameter);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(AdapterFaultReport))]
        [TransactionFlow(TransactionFlowOption.Allowed)] // some comment
        Result CreateOperation();
    }
}