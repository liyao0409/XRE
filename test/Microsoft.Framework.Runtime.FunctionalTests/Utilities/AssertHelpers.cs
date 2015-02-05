// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Microsoft.Framework.Runtime.FunctionalTests.Utilities
{
    public static class AssertHelpers
    {
        public static void SortAndEqual<T>(IEnumerable<T> expect, IEnumerable<T> actual, IComparer<T> comparer)
        {
            var expectArray = expect.OrderBy(elem => elem, comparer).ToArray();
            var actualArray = actual.OrderBy(elem => elem, comparer).ToArray();

            var missing = new List<T>();
            var extra = new List<T>();

            int i = 0, j = 0;
            while (i < expectArray.Length && j < actualArray.Length)
            {
                var compareResult = comparer.Compare(expectArray[i], actualArray[j]);
                if (compareResult < 0)
                {
                    missing.Add(expectArray[i]);
                    ++i;
                }
                else if (compareResult == 0)
                {
                    ++i;
                    ++j;
                }
                else
                {
                    extra.Add(actualArray[j]);
                    ++j;
                }
            }

            var pass = true;
            var errorMessage = new StringBuilder("Two collections are not equal");

            if (i < expectArray.Length)
            {
                missing.AddRange(expectArray.Skip(i));
            }

            if (j < actualArray.Length)
            {
                extra.AddRange(actualArray.Skip(j));
            }

            if (missing.Any())
            {
                pass = false;
                errorMessage.Append("\nMissing:\n");
                foreach (var missedElement in missing)
                {
                    errorMessage.AppendFormat("\t{0}\n", missedElement.ToString());
                }
            }

            if (extra.Any())
            {
                pass = false;
                errorMessage.Append("\nExtra:\n");
                foreach (var extraElement in extra)
                {
                    errorMessage.AppendFormat("\t{0}\n", extraElement.ToString());
                }
            }

            Assert.True(pass, errorMessage.ToString());
        }
    }
}