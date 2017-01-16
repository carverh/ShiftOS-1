/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Discoursistency.HTTP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discoursistency.HTTP.Client.Models;

/// <summary>
/// Shift OS auth agent.
/// </summary>
namespace ShiftOS.Objects
{
	/// <summary>
	/// Shift OS auth agent.
	/// </summary>
    public class ShiftOSAuthAgent : Discoursistency.Base.Authentication.DiscourseAuthenticationService
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ShiftOS.Objects.ShiftOSAuthAgent"/> class.
		/// </summary>
		/// <param name="client">Client.</param>
        public ShiftOSAuthAgent(IClient client) : base(client)
        {
			// TODO: Fill this out
        }
    }

}

// Comments by @carverh
