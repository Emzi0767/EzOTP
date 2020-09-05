// This file is part of RosettaCTF project.
//
// Copyright 2020 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EzOTP.Internal
{
    internal sealed class EnumNameConverter
    {
        public static EnumNameConverter Instance { get; } = new EnumNameConverter();

        private readonly IDictionary<Type, Definition> _definitions;

        public EnumNameConverter()
        {
            this._definitions = new Dictionary<Type, Definition>();
        }

        public bool TryConvert<T>(string name, out T value)
            where T : Enum
        {
            var t = typeof(T);
            if (!this._definitions.TryGetValue(t, out var def))
            {
                var vals = t.GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.FieldType == t)
                    .Select(x => new { value = (T)x.GetValue(null), names = x.GetCustomAttribute<EnumNameAttribute>()?.Names })
                    .Select(x => new { x.value, names = x.names ?? new[] { x.value.ToString() } })
                    .SelectMany(x => x.names.Select(y => (x.value, y)));

                def = new Definition<T>(vals);
                this._definitions.TryAdd(t, def);
            }

            var deft = def as Definition<T>;
            return deft.TryConvert(name, out value);
        }

        private class Definition
        { }

        private sealed class Definition<T> : Definition
            where T : Enum
        {
            private readonly IReadOnlyDictionary<string, T> _stMap;

            public Definition(IEnumerable<(T value, string name)> pairs)
            {
                this._stMap = pairs.ToDictionary(x => x.name, x => x.value, StringComparer.OrdinalIgnoreCase);
            }

            public bool TryConvert(string name, out T value)
                => this._stMap.TryGetValue(name, out value);
        }
    }
}
