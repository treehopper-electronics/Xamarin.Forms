using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Xamarin.Forms.Build.Tasks
{
	static class ModuleDefinitionExtensions
	{
		static Dictionary<(ModuleDefinition module, string typeKey), TypeReference> TypeRefCache = new Dictionary<(ModuleDefinition module, string typeKey), TypeReference>();
		public static TypeReference ImportReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type)
		{
			var typeKey = type.ToString();
			if (!TypeRefCache.TryGetValue((module, typeKey), out var typeRef))
				TypeRefCache.Add((module, typeKey), typeRef = module.ImportReference(module.GetTypeDefinition(type)));
			return typeRef;
		}

		public static TypeReference ImportReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, (string assemblyName, string clrNamespace, string typeName)[] classArguments)
		{
			var typeKey = $"{type}<{string.Join(",",classArguments)}>";
			if (!TypeRefCache.TryGetValue((module, typeKey), out var typeRef))
				TypeRefCache.Add((module, typeKey), typeRef = module.ImportReference(module.ImportReference(type).MakeGenericInstanceType(classArguments.Select(gp => module.GetTypeDefinition((gp.assemblyName, gp.clrNamespace, gp.typeName))).ToArray())));
			return typeRef;
		}

		public static TypeReference ImportArrayReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type)
		{
			var typeKey = "${type}[]";
			if (!TypeRefCache.TryGetValue((module, typeKey), out var typeRef))
				TypeRefCache.Add((module, typeKey), typeRef = module.ImportReference(module.ImportReference(type).MakeArrayType()));
			return typeRef;
		}

		static Dictionary<(ModuleDefinition module, string ctorKey), MethodReference> CtorRefCache = new Dictionary<(ModuleDefinition module, string ctorKey), MethodReference>();
		static MethodReference ImportCtorReference(this ModuleDefinition module, TypeReference type, TypeReference[] classArguments, Func<MethodDefinition, bool> predicate)
		{
			var ctor = module.ImportReference(type).ResolveCached().Methods.FirstOrDefault(md => !md.IsPrivate && !md.IsStatic && md.IsConstructor && (predicate?.Invoke(md) ?? true));
			if (ctor is null)
				return null;
			var ctorRef = module.ImportReference(ctor);
			if (classArguments == null)
				return ctorRef;
			return module.ImportReference(ctorRef.ResolveGenericParameters(type.MakeGenericInstanceType(classArguments), module));
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, TypeReference type, TypeReference[] parameterTypes)
		{
			var ctorKey = $"{type}.ctor({(parameterTypes == null ? "" : string.Join(",", parameterTypes.Select(tr => (tr.Module.Assembly.Name.Name, tr.Namespace, tr.Name))))})";
			if (CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				return ctorRef;
			ctorRef = module.ImportCtorReference(type, classArguments: null, predicate: md => {
				if (md.Parameters.Count != (parameterTypes?.Length ?? 0))
					return false;
				for (var i = 0; i < md.Parameters.Count; i++)
					if (!TypeRefComparer.Default.Equals(md.Parameters[i].ParameterType, module.ImportReference(module.ImportReference(parameterTypes[i]))))
						return false;
				return true;
			});
			CtorRefCache.Add((module, ctorKey), ctorRef);
			return ctorRef;
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, int paramCount)
		{
			var ctorKey = $"{type}.ctor({(string.Join(",", Enumerable.Repeat("_", paramCount)))})";
			if (!CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				CtorRefCache.Add((module, ctorKey), ctorRef = module.ImportCtorReference(module.GetTypeDefinition(type), null, md => md.Parameters.Count == paramCount));
			return ctorRef;
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, TypeReference type, int paramCount)
		{
			var ctorKey = $"{type}.ctor({(string.Join(",", Enumerable.Repeat("_", paramCount)))})";
			if (!CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				CtorRefCache.Add((module, ctorKey), ctorRef = module.ImportCtorReference(type, null, md => md.Parameters.Count == paramCount));
			return ctorRef;
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, int paramCount, (string assemblyName, string clrNamespace, string typeName)[] classArguments)
		{
			var ctorKey = $"{type}<{(string.Join(",", classArguments))}>.ctor({(string.Join(",", Enumerable.Repeat("_", paramCount)))})";
			if (!CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				CtorRefCache.Add((module, ctorKey), ctorRef = module.ImportCtorReference(module.GetTypeDefinition(type), classArguments.Select(module.GetTypeDefinition).ToArray(), md=>md.Parameters.Count==paramCount));
			return ctorRef;
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, int paramCount, TypeReference[] classArguments)
		{
			var ctorKey = $"{type}<{(string.Join(",", classArguments.Select(tr => (tr.Module.Assembly.Name.Name, tr.Namespace, tr.Name))))}>.ctor({(string.Join(",", Enumerable.Repeat("_", paramCount)))})";
			if (!CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				CtorRefCache.Add((module, ctorKey), ctorRef = module.ImportCtorReference(module.GetTypeDefinition(type), classArguments, predicate: md => md.Parameters.Count == paramCount));
			return ctorRef;
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, (string assemblyName, string clrNamespace, string typeName)[] parameterTypes, (string assemblyName, string clrNamespace, string typeName)[] classArguments)
		{
			var ctorKey = $"{type}<{(string.Join(",", classArguments))}>.ctor({(parameterTypes == null ? "" : string.Join(",", parameterTypes))})";
			if (CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				return ctorRef;
			ctorRef = module.ImportCtorReference(module.GetTypeDefinition(type), classArguments.Select(module.GetTypeDefinition).ToArray(), md => {
				if (md.Parameters.Count != (parameterTypes?.Length ?? 0))
					return false;
				for (var i = 0; i < md.Parameters.Count; i++)
					if (!TypeRefComparer.Default.Equals(md.Parameters[i].ParameterType, module.ImportReference(module.ImportReference(parameterTypes[i]))))
						return false;
				return true;
			});
			CtorRefCache.Add((module, ctorKey), ctorRef);
			return ctorRef;
		}

		public static MethodReference ImportCtorReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, (string assemblyName, string clrNamespace, string typeName)[] parameterTypes)
		{
			var ctorKey = $"{type}.ctor({(parameterTypes == null ? "" : string.Join(",", parameterTypes))})";
			if (CtorRefCache.TryGetValue((module, ctorKey), out var ctorRef))
				return ctorRef;
			ctorRef = module.ImportCtorReference(module.GetTypeDefinition(type), classArguments: null, predicate: md => {
				if (md.Parameters.Count != (parameterTypes?.Length ?? 0))
					return false;
				for (var i = 0; i < md.Parameters.Count; i++)
					if (!TypeRefComparer.Default.Equals(md.Parameters[i].ParameterType, module.ImportReference(module.ImportReference(parameterTypes[i]))))
						return false;
				return true;
			});
			CtorRefCache.Add((module, ctorKey), ctorRef);
			return ctorRef;
		}

		public static MethodReference ImportPropertyGetterReference(this ModuleDefinition module, TypeReference type, string propertyName, Func<PropertyDefinition, bool> predicate = null, bool flatten = false)
		{
			var properties = module.ImportReference(type).Resolve().Properties;
			var getter = module
				.ImportReference(type)
				.ResolveCached()
				.Properties(flatten)
				.FirstOrDefault(pd =>
								   pd.Name == propertyName
								&& (predicate?.Invoke(pd) ?? true))
				?.GetMethod;
			return getter == null ? null : module.ImportReference(getter);
		}

		public static MethodReference ImportPropertyGetterReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, string propertyName, Func<PropertyDefinition, bool> predicate = null, bool flatten = false)
		{
			return module.ImportPropertyGetterReference(module.GetTypeDefinition(type), propertyName, predicate, flatten);
		}

		public static MethodReference ImportPropertySetterReference(this ModuleDefinition module, TypeReference type, string propertyName, Func<PropertyDefinition, bool> predicate = null)
		{
			var setter = module
				.ImportReference(type)
				.ResolveCached()
				.Properties
				.FirstOrDefault(pd =>
								   pd.Name == propertyName
								&& (predicate?.Invoke(pd) ?? true))
				?.SetMethod;
			return setter == null ? null : module.ImportReference(setter);
		}

		public static MethodReference ImportPropertySetterReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, string propertyName, Func<PropertyDefinition, bool> predicate = null)
		{
			return module.ImportPropertySetterReference(module.GetTypeDefinition(type), propertyName, predicate);
		}

		public static FieldReference ImportFieldReference(this ModuleDefinition module, TypeReference type, string fieldName, Func<FieldDefinition, bool> predicate = null)
		{
			var field = module
				.ImportReference(type)
				.ResolveCached()
				.Fields
				.FirstOrDefault(fd =>
								   fd.Name == fieldName
								&& (predicate?.Invoke(fd) ?? true));
			return field == null ? null : module.ImportReference(field);
		}

		public static FieldReference ImportFieldReference(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type, string fieldName, Func<FieldDefinition, bool> predicate = null)
		{
			return module.ImportFieldReference(module.GetTypeDefinition(type), fieldName: fieldName, predicate: predicate);
		}

		public static MethodReference ImportMethodReference(this ModuleDefinition module, TypeReference type, string methodName, int paramCount, Func<MethodDefinition, bool> predicate = null, TypeReference[] classArguments = null)
		{
			var method = module
				.ImportReference(type)
				.ResolveCached()
				.Methods
				.FirstOrDefault(md =>
								   !md.IsConstructor
								&& md.Name == methodName
								&& md.Parameters.Count == paramCount
								&& (predicate?.Invoke(md) ?? true));
			if (method is null)
				return null;
			var methodRef = module.ImportReference(method);
			if (classArguments == null)
				return methodRef;
			return module.ImportReference(methodRef.ResolveGenericParameters(type.MakeGenericInstanceType(classArguments), module));
		}

		public static MethodReference ImportMethodReference(this ModuleDefinition module,
															(string assemblyName, string clrNamespace, string typeName) type,
															string methodName, int paramCount, Func<MethodDefinition, bool> predicate = null,
															(string assemblyName, string clrNamespace, string typeName)[] classArguments = null)
		{
			return module.ImportMethodReference(module.GetTypeDefinition(type), methodName, paramCount, predicate,
												classArguments?.Select(gp => module.GetTypeDefinition((gp.assemblyName, gp.clrNamespace, gp.typeName))).ToArray());
		}

		static Dictionary<(ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName)), TypeDefinition> typeDefCache
			= new Dictionary<(ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName)), TypeDefinition>();

		public static TypeDefinition GetTypeDefinition(this ModuleDefinition module, (string assemblyName, string clrNamespace, string typeName) type)
		{
			if (typeDefCache.TryGetValue((module, type), out TypeDefinition cachedTypeDefinition))
				return cachedTypeDefinition;

			var asm = module.Assembly.Name.Name == type.assemblyName
							? module.Assembly
							: module.AssemblyResolver.Resolve(AssemblyNameReference.Parse(type.assemblyName));
			var typeDef = asm.MainModule.GetType($"{type.clrNamespace}.{type.typeName}");
			if (typeDef != null) {
				typeDefCache.Add((module, type), typeDef);
				return typeDef;
			}
			var exportedType = asm.MainModule.ExportedTypes.FirstOrDefault(
				arg => arg.IsForwarder && arg.Namespace == type.clrNamespace && arg.Name == type.typeName);
			if (exportedType != null) {
				typeDef = exportedType.Resolve();
				typeDefCache.Add((module, type), typeDef);
				return typeDef;
			}

			//I hate you, netstandard
			if (type.assemblyName == "mscorlib" && type.clrNamespace == "System.Reflection")
				return module.GetTypeDefinition(("System.Reflection", type.clrNamespace, type.typeName));
			return null;
		}

		static IEnumerable<PropertyDefinition> Properties(this TypeDefinition typedef, bool flatten)
		{
			foreach (var property in typedef.Properties)
				yield return property;
			if (!flatten || typedef.BaseType == null)
				yield break;
			foreach (var property in typedef.BaseType.ResolveCached().Properties(true))
				yield return property;
		}
	}
}