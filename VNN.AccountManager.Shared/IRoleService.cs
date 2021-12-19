using System.ServiceModel;
using System.Windows.Markup;
using NG.AccountCommands.Commands;
using NG.AccountCommands.Queries;
using NG.AccountReadModels;
using NG.BaseCommands;

namespace VNN.AccountManager.Shared;

[ServiceContract]
public interface IRoleService
{
    [OperationContract]
    Task<BaseCommandResponse<RRole[]>> GetAll();

    [OperationContract]
    Task<BaseCommandResponse<RRole>> GetById(RoleByIdQuery query);

    [OperationContract]
    Task<BaseCommandResponse<RRole[]>> Gets(RoleGetsQuery query);

    [OperationContract]
    Task<BaseCommandResponse<RRole[]>> GetByIds(string[] ids);

    [OperationContract]
    Task<BaseCommandResponse<RActionDefine[]>> ActionDefineGets(ActionDefineGetsQuery query);

    [OperationContract]
    Task<BaseCommandResponse<RRoleActionMapping[]>> GetRoleActionMappingByRoleId(string roleId);

    [OperationContract]
    Task<BaseCommandResponse> Add(RoleAddCommand command);

    [OperationContract]
    Task<BaseCommandResponse> Change(RoleChangeCommand command);

    [OperationContract]
    Task<BaseCommandResponse> RoleActionDefineChange(RoleActionDefineMappingChangeCommand command);

    [OperationContract]
    Task<BaseCommandResponse> RoleActionDefineAdminChange(RoleActionDefineMappingAdminChangeCommand command);

    [OperationContract]
    Task<BaseCommandResponse<RActionDefine[]>> ActionDefineGetByIds(string[] ids);

    [OperationContract]
    Task<BaseCommandResponse> ActionDefineAdd(ActionDefineAddCommand command);
}