// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Result;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

internal class ResultService : Grpc.ResultService.ResultServiceBase
{
   #region Constants and Fields

   private readonly ResultReporter resultReporter;

   #endregion

   #region Constructors and Destructors

   public ResultService(ResultReporter resultReporter)
   {
      this.resultReporter = resultReporter ?? throw new ArgumentNullException(nameof(resultReporter));
   }

   #endregion

   #region Public Methods and Operators

   public override async Task ResultChanged(ResultChangedRequest request, IServerStreamWriter<ResultChangedResponse> responseStream,
      ServerCallContext context)
   {
      var resultInfo = await resultReporter.GetResultAsync();
      var response = CreateResponse(resultInfo);
      await responseStream.WriteAsync(response);
   }

   private static ResultChangedResponse CreateResponse(ResultInfo resultInfo)
   {
      var response = new ResultChangedResponse { ExitCode = resultInfo.ExitCode, Message = resultInfo.Message };
      if (resultInfo.Data.Count > 0)
         response.Data.Add(resultInfo.Data);
      return response;
   }

   #endregion
}