﻿#region File header
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolUnitOfExecution.cs" company="" product="Michonne">
//   Copyright 2014 Cyrille Dupuydauby
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion
namespace Michonne.Implementation
{
    using System;
    using System.Threading;

    using Michonne.Interfaces;

    /// <summary>
    /// The pool unit of execution executes submitted <see cref="Action"/> through the CLR Pool.
    /// </summary>
    public class PoolUnitOfExecution : IUnitOfExecution
    {
        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to be eventually executed.</param>
        public void Dispatch(Action action)
        {
            ThreadPool.QueueUserWorkItem((x) => ((Action)x)(), action);
        }
    }
}