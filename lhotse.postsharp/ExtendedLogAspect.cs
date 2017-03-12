using System;
using System.Diagnostics;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace lhotse.postsharp
{
    [PSerializable]
    public class ExtendedLogAspect : OnMethodBoundaryAspect
    {
        private string _methodName;
        private Type _className;
        private DateTime _startTime;

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            _methodName = method.Name;
            _className = method.DeclaringType;
        }

        public sealed override void OnEntry(MethodExecutionArgs args)
        {
            Trace.WriteLine($"[{DateTime.Now}][ENTRY][{_className}.{_methodName}]:");
            _startTime = DateTime.Now;
            base.OnEntry(args);
        }

        public sealed override void OnExit(MethodExecutionArgs args)
        {
            Trace.WriteLine($"[{DateTime.Now}][EXIT][{_className}.{_methodName}]: {(DateTime.Now-_startTime).Milliseconds} ms");
            base.OnExit(args);
        }

        public sealed override void OnException(MethodExecutionArgs args)
        {
            Trace.WriteLine($"[{DateTime.Now}][EXCEPTION][{_className}.{_methodName}]: Message:{args.Exception.Message}");
            Trace.WriteLine($"Stack trace:{args.Exception.StackTrace}");
            base.OnException(args);
        }
    }
}