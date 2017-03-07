﻿using System;
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

        public override void OnEntry(MethodExecutionArgs args)
        {
            Trace.WriteLine($"[{DateTime.Now}] {_className}.{_methodName}: Start:");
            base.OnEntry(args);
            _startTime = DateTime.Now;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Trace.WriteLine($"[{DateTime.Now}] {_className}.{_methodName}: End: {(DateTime.Now-_startTime).Milliseconds}");
            base.OnExit(args);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Trace.WriteLine($"[{DateTime.Now}] {_className}.{_methodName}: Exception: {args.Exception.Message}");
            base.OnException(args);
        }
    }
}