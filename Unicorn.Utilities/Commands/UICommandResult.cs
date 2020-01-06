using System;

namespace Unicorn.Utilities.Commands
{
        public class UICommandResult
        {
                public UICommandResultStatus ResultStatus
                {
                        get;
                }
                public object Result
                {
                        get;
                }
                public string ErrMsg
                {
                        get;
                }
                public Exception Exception
                {
                        get;
                }

                public UICommandResult(UICommandResultStatus resultStatus)
                        : this(resultStatus, null, null, null)
                {
                }

                public UICommandResult(UICommandResultStatus resultStatus, object result)
                           : this(resultStatus, result, null, null)
                {
                }

                public UICommandResult(UICommandResultStatus resultStatus, string errMsg)
                           : this(resultStatus, null, errMsg, null)
                {
                }

                public UICommandResult(UICommandResultStatus resultStatus, object result, string errMsg, Exception exception)
                {
                        ResultStatus = resultStatus;
                        Result = result;
                        ErrMsg = errMsg;
                        Exception = exception;
                }
        }
}
