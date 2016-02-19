﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.32559
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.VisualStudio.ServiceReference.Platforms, version 12.0.20617.1
// 
namespace GoG.WinRT80.FuegoService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FuegoService.IFuegoService")]
    public interface IFuegoService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/GetGameExists", ReplyAction="http://tempuri.org/IFuegoService/GetGameExistsResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(GoG.Infrastructure.Services.Engine.GoGameStateResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(GoG.Infrastructure.Services.Engine.GoMoveResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(GoG.Infrastructure.Services.Engine.GoHintResponse))]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoResponse> GetGameExistsAsync(System.Guid gameid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/GetGameState", ReplyAction="http://tempuri.org/IFuegoService/GetGameStateResponse")]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoGameStateResponse> GetGameStateAsync(System.Guid gameid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/Start", ReplyAction="http://tempuri.org/IFuegoService/StartResponse")]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoGameStateResponse> StartAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoGameState state);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/GenMove", ReplyAction="http://tempuri.org/IFuegoService/GenMoveResponse")]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoMoveResponse> GenMoveAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoColor color);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/Play", ReplyAction="http://tempuri.org/IFuegoService/PlayResponse")]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoMoveResponse> PlayAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoMove move);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/Undo", ReplyAction="http://tempuri.org/IFuegoService/UndoResponse")]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoGameStateResponse> UndoAsync(System.Guid gameid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFuegoService/Hint", ReplyAction="http://tempuri.org/IFuegoService/HintResponse")]
        System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoHintResponse> HintAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoColor color);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFuegoServiceChannel : GoG.WinRT80.FuegoService.IFuegoService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FuegoServiceClient : System.ServiceModel.ClientBase<GoG.WinRT80.FuegoService.IFuegoService>, GoG.WinRT80.FuegoService.IFuegoService {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public FuegoServiceClient() : 
                base(FuegoServiceClient.GetDefaultBinding(), FuegoServiceClient.GetDefaultEndpointAddress()) {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IFuegoService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public FuegoServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(FuegoServiceClient.GetBindingForEndpoint(endpointConfiguration), FuegoServiceClient.GetEndpointAddress(endpointConfiguration)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public FuegoServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(FuegoServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public FuegoServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(FuegoServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public FuegoServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoResponse> GetGameExistsAsync(System.Guid gameid) {
            return base.Channel.GetGameExistsAsync(gameid);
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoGameStateResponse> GetGameStateAsync(System.Guid gameid) {
            return base.Channel.GetGameStateAsync(gameid);
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoGameStateResponse> StartAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoGameState state) {
            return base.Channel.StartAsync(gameid, state);
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoMoveResponse> GenMoveAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoColor color) {
            return base.Channel.GenMoveAsync(gameid, color);
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoMoveResponse> PlayAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoMove move) {
            return base.Channel.PlayAsync(gameid, move);
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoGameStateResponse> UndoAsync(System.Guid gameid) {
            return base.Channel.UndoAsync(gameid);
        }
        
        public System.Threading.Tasks.Task<GoG.Infrastructure.Services.Engine.GoHintResponse> HintAsync(System.Guid gameid, GoG.Infrastructure.Engine.GoColor color) {
            return base.Channel.HintAsync(gameid, color);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IFuegoService)) {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IFuegoService)) {
                return new System.ServiceModel.EndpointAddress("http://localhost/GoG.Services/Fuego.svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding() {
            return FuegoServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_IFuegoService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() {
            return FuegoServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IFuegoService);
        }
        
        public enum EndpointConfiguration {
            
            BasicHttpBinding_IFuegoService,
        }
    }
}