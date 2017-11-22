using Constellation.Models;
using Constellation.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Constellation.Package
{
    public class PackageHost
    {

        //
        // Summary:
        //     Retry connection interval (3 seconds)
        public const int RetryConnectionInterval = 3000;
        /// <summary>
        /// Maximum retry before reseting subscription ID for messages and states objects. It may mean we lost the connection and need a new key
        /// </summary>
        private const int RetryMaxError = 60;
        static private int RetryStateObjects = 0;
        static private int RetryMessages = 0;

        #region package definition        
        //
        // Summary:
        //     Gets/Sets the name of the Constellation server.
        public static string ServerName { get; set; }
        //
        // Summary:
        //     Gets/Sets the port of the Constellation server.
        public static string Port { get; set; }
        //
        // Summary:
        //     Gets/Sets if https is used or not.
        public static bool isHttps { get; set; }
        //
        // Summary:
        //     Gets/Sets the name of the current package.
        public static string PackageName { get; set; }

        //
        // Summary:
        //     Gets/Sets the name of the current sentinel
        public static string SentinelName { get; set; }
        //
        // Summary:
        //     Gets/Sets the access key
        public static string AccessKey { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether this package is connected to the Constellation's
        //     hub.
        public static bool IsConnected
        {
            get
            {
                //using the http://localhost:8088/rest/constellation/CheckAccess?SentinelName=MyVirtualSentinel&PackageName=MyVirtualPackage&AccessKey=MyAccessKey
                try
                {
                    var ret = RunRequest("CheckAccess");
                    if ((ret != null) && (bIsAllInitialzed))
                        return true;
                    else return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UPS: {ex.Message}");
                    return false;
                }
            }
        }

        static private bool bIsAllInitialzed = false;

        //
        // Summary:
        //     Gets a value indicating whether this package is running.
        public static bool IsRunning
        {
            get
            {
                //if we have the right to access, then say it's connected
                return IsConnected & bIsAllInitialzed;
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether this package auto reconnect to the hub.
        public static bool AutoReconnect { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether this package is stand alone.
        public static bool IsStandAlone
        {
            get
            {
                //All packages like this are stand alone
                return true;
            }
        }

        //
        // Summary:
        //     Gets a value indicating whether this instance has control manager.
        public static bool HasControlManager
        {
            get
            {
                //always return false for those kind of packages
                return false;
            }
        }

        //
        // Summary:
        //     Gets the current package.
        public static IPackage Package { get; internal set; }
        private static Type packageType;
        //
        // Summary:
        //     Gets/Sets the package descriptor.
        public static PackageDescriptor PackageDescriptor { get; set; }


        //
        // Summary:
        //     Declares the Package descriptor on Constellation Server.
        public static void DeclarePackageDescriptor()
        {
            // DeclarePackageDescriptor
            try
            {

                RunRequest("DeclarePackageDescriptor", JsonConvert.SerializeObject(PackageDescriptor), false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }

        }
        #endregion

        #region events
        //
        // Summary:
        //     Occurs when the settings is updated.
        public static event EventHandler SettingsUpdated;
        //
        // Summary:
        //     Occurs when state object updated.
        public static event EventHandler<StateObjectUpdatedEventArgs> StateObjectUpdated;
        //
        // Summary:
        //     Occurs when the server send the last StateObject's list of your package.
        public static event EventHandler<StateObjectsListEventArgs> LastStateObjectsReceived;
        #endregion

        #region settings

        private static Dictionary<string, string> packgeSettings = new Dictionary<string, string>();
        //
        // Summary:
        //     Gets the package settings.
        public static Dictionary<string, string> Settings { get { return packgeSettings; } }

        //
        // Summary:
        //     Request the settings from constellation.
        //
        // Returns:
        //     Nothing, get the settings thru the Settings or GetSettingValue.
        public static void RequestSettings()
        {
            try
            {
                var ret = RunRequest("GetSettings");
                if (ret == null)
                    return;
                packgeSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
                SettingsUpdated?.Invoke(null, null);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }

        //
        // Summary:
        //     Gets the setting value from JSON.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   throwException:
        //     if set to true [throw exception].
        //
        // Returns:
        //     The deserialized object from the setting JSON value.
        public static dynamic GetSettingAsJsonObject(string key, bool throwException = false)
        {
            try
            {
                return JsonConvert.DeserializeObject(GetSettingValue<string>(key));
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw ex;
                }
                return null;
            }
        }
        //
        // Summary:
        //     Gets the setting value from JSON.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   throwException:
        //     if set to true [throw exception].
        //
        // Type parameters:
        //   T:
        //     The type of the object to deserialize to.
        //
        // Returns:
        //     The deserialized object from the setting JSON value.
        public static T GetSettingAsJsonObject<T>(string key, bool throwException = false)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(GetSettingValue<string>(key));
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw ex;
                }
                return default(T);
            }
        }
        //
        // Summary:
        //     Gets the XmlDocument of the setting.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   throwException:
        //     if set to true [throw exception].
        //
        // Returns:
        //     The XmlDocument of the setting.
        public static XmlDocument GetSettingAsXmlDocument(string key, bool throwException = false)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(GetSettingValue<string>(key));
                return xmlDocument;
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw ex;
                }
                return null;
            }
        }
        //
        // Summary:
        //     Gets the setting value as T.
        //
        // Parameters:
        //   key:
        //     The key of the setting.
        //
        // Type parameters:
        //   T:
        //     Type of the setting.
        //
        // Returns:
        //     The setting value as T
        public static T GetSettingValue<T>(string key)
        {
            return ((T)Convert.ChangeType(Settings[key], typeof(T)));
        }

        //
        // Summary:
        //     Determines whether the package's settings contains the specified key.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        // Returns:
        //     true if the setting with the specified key exists; otherwise, false.
        public static string GetSettingValue(string key)
        {
            return packgeSettings[key];
        }

        //
        // Summary:
        //     Determines whether the package's settings contains the specified key.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        // Returns:
        //     true if the setting with the specified key exists; otherwise, false.
        public static bool ContainsSetting(string key)
        {
            return (packgeSettings.ContainsKey(key));
        }
        #endregion

        #region StateObject

        #region Push state objects
        // sotre all the state objects already got from the server
        private static List<StateObject> lastStateObject = new List<StateObject>();
        // what are the state objects to check for changes
        // To store objects and states which needs to be notified
        static private List<PropertyRaiseEvent> PropertyToRaise = new List<PropertyRaiseEvent>();

        //
        // Summary:
        //     Purges the state objects.
        //
        // Parameters:
        //   name:
        //     The StateObject name.
        //
        //   type:
        //     The type.
        public static void PurgeStateObjects(string name = "*", string type = "*")
        {
            //http://localhost:8088/rest/constellation/PurgeStateObjects?SentinelName=MyVirtualSentinel&PackageName=MyVirtualPackage&AccessKey=MyAccessKey&name=Demo
            try
            {
                RunRequest("PurgeStateObjects", $"name={name}&type={type}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }

        //
        // Summary:
        //     Pushes the state object.
        //
        // Parameters:
        //   name:
        //     The name of the state object.
        //
        //   value:
        //     The value of the state object.
        //
        //   type:
        //     The type (optionnal: if empty the type is the Full type name of the value).
        //
        //   metadatas:
        //     The metadatas.
        //
        //   lifetime:
        //     The number of seconds to expire the StateObjects. 0 for no expiration.
        public static void PushStateObject(string name, object value, string type = "", Dictionary<string, object> metadatas = null, int lifetime = 0)
        {
            //http://localhost:8088/rest/constellation/PushStateObject?SentinelName=MyVirtualSentinel&PackageName=MyVirtualPackage&AccessKey=MyAccessKey&name=Temperature&value=21
            try
            {
                PostStateObject stateObject = new PostStateObject();
                stateObject.Lifetime = lifetime;
                stateObject.Type = type;
                stateObject.Name = name;
                stateObject.Value = value;
                stateObject.Metadatas = metadatas;
                RunRequest("PushStateObject", JsonConvert.SerializeObject(stateObject), false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }

        }
        #endregion

        #region pull and subscribe to state objects

        private static string subscriptionId = "";

        public static void RequestStateObjects(string sentinel = "*", string package = "*", string name = "*", string type = "*")
        {
            //http://localhost:8088/rest/constellation/RequestStateObjects?SentinelName=Consumer&PackageName=Demo&AccessKey=MaCleDeTest123&package=HWMonitor&name=/intelcpu/load/0
            try
            {
                string param = $"sentinel={sentinel}&package={package}&name={name}&type={type}";
                var ret = RunRequest("RequestStateObjects", param);
                if (ret == null)
                    return;
                List<StateObject> stateObject = JsonConvert.DeserializeObject<List<StateObject>>(ret);
                CheckStateObjectsAndNotify(stateObject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }

        static private void CheckStateObjectsAndNotify(List<StateObject> stateObject)
        {
            bIsCheckingState = true;
            foreach (var so in stateObject)
            {
                CheckStateObjectsAndNotify(so);
            }

            EventHandler<StateObjectsListEventArgs> handlerLast = LastStateObjectsReceived;
            if (handlerLast != null)
            {
                StateObjectsListEventArgs stateObjectUpdatedEventArgs = new StateObjectsListEventArgs();
                stateObjectUpdatedEventArgs.StateObjects = lastStateObject;
                handlerLast(null, stateObjectUpdatedEventArgs);
            }
            bIsCheckingState = false;
        }

        static private void CheckStateObjectsAndNotify(StateObject so)
        {
            foreach (var StateToCheck in PropertyToRaise)
            {    //check the states of the subscriptions
                if (((StateToCheck.StateObjectsToCheck.Name == so.Name) || (StateToCheck.StateObjectsToCheck.Name == "*")) &&
                    ((StateToCheck.StateObjectsToCheck.Sentinel == so.SentinelName) || (StateToCheck.StateObjectsToCheck.Sentinel == "*")) &&
                    ((StateToCheck.StateObjectsToCheck.Package == so.PackageName) || (StateToCheck.StateObjectsToCheck.Package == "*")) &&
                    ((StateToCheck.StateObjectsToCheck.Type == so.Type) || (StateToCheck.StateObjectsToCheck.Type == "*")))
                {
                    // check if an old state exist
                    if (lastStateObject.Exists(m =>
                     ((m.Name == so.Name) || (StateToCheck.StateObjectsToCheck.Name == "*")) &&
                     ((m.SentinelName == so.SentinelName) || (StateToCheck.StateObjectsToCheck.Sentinel == "*")) &&
                     ((m.PackageName == so.PackageName) || (StateToCheck.StateObjectsToCheck.Package == "*")) &&
                     ((m.Type == so.Type) || (StateToCheck.StateObjectsToCheck.Type == "*"))))
                    {
                        // select the old state
                        var old = lastStateObject.Find(m =>
                            ((m.Name == so.Name) || (StateToCheck.StateObjectsToCheck.Name == "*")) &&
                            ((m.SentinelName == so.SentinelName) || (StateToCheck.StateObjectsToCheck.Sentinel == "*")) &&
                            ((m.PackageName == so.PackageName) || (StateToCheck.StateObjectsToCheck.Package == "*")) &&
                            ((m.Type == so.Type) || (StateToCheck.StateObjectsToCheck.Type == "*")));
                        // if the value has changed
                        if ((old.LastUpdate != so.LastUpdate) && (old.Value.ToString() != so.Value.ToString()))
                        {
                            RaiseEvents(StateToCheck, so, old);
                        }
                    }
                    // state object was not exiusting, raise an event
                    else
                    {
                        if (StateToCheck.StateObjectsToCheck.RequestValueOnInit)
                            RaiseEvents(StateToCheck, so, null);
                    }
                }
            }
            // replace the new state by the old states
            if (lastStateObject.Exists(m =>
                ((m.Name == so.Name)) &&
                ((m.SentinelName == so.SentinelName)) &&
                ((m.PackageName == so.PackageName)) &&
                ((m.Type == so.Type))))
            {
                var old = lastStateObject.FindIndex(m =>
                ((m.Name == so.Name)) &&
                ((m.SentinelName == so.SentinelName)) &&
                ((m.PackageName == so.PackageName)) &&
                ((m.Type == so.Type)));
                lastStateObject[old] = so;
            }
            else
            {
                lastStateObject.Add(so);
            }
        }

        static private void RaiseEvents(PropertyRaiseEvent StateToCheck, StateObject newstate, StateObject oldstate)
        {
            // case it's a StateObjectAttribute
            if (StateToCheck.Property != null)
            {
                var prop = StateToCheck;
                {
                    // Raise events for the ValueChanged
                    var evtch = prop.Property.PropertyType.GetField("ValueChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                    var toinvoke = (MulticastDelegate)evtch.GetValue(prop.StateNotifyer);
                    if (toinvoke != null)
                    {
                        StateObjectChangedEventArgs args = new StateObjectChangedEventArgs();
                        args.OldState = oldstate;
                        args.NewState = newstate;
                        // raise event for all the registered events
                        foreach (var handler in toinvoke.GetInvocationList())
                        {
                            handler.Method.Invoke(handler.Target, new object[] { null, args });
                        }
                    }
                    //Raise events for the PropertyChanged
                    evtch = prop.Property.PropertyType.GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                    toinvoke = (MulticastDelegate)evtch.GetValue(prop.StateNotifyer);
                    if (toinvoke != null)
                    {
                        foreach (var handler in toinvoke.GetInvocationList())
                        {
                            handler.Method.Invoke(handler.Target, new object[] { null, new PropertyChangedEventArgs("Value") });
                        }

                    }
                }
            }
            // case it's a normal subscription
            else
            {
                // if initial value does not need to be rased as an event
                if (oldstate == null)
                    if (!StateToCheck.StateObjectsToCheck.RequestValueOnInit)
                        return;
                // call the function
                StateToCheck.ActionState(newstate);
            }
        }


        //
        // Summary:
        //     Registers the StateObject callback on update.
        //
        // Parameters:
        //   callBack:
        //     The callback.
        //
        //   sentinel:
        //     The sentinel.
        //
        //   package:
        //     The package.
        //
        //   name:
        //     The StateObject name.
        //
        //   type:
        //     The type.
        //
        //   requestValueOnInit:
        //     if set to true request value on initialize.
        public static void RegisterStateObjectCallback(Action<StateObject> callBack, string sentinel = "*", string package = "*", string name = "*", string type = "*", bool requestValueOnInit = true)
        {
            //http://localhost:8088/rest/constellation/SubscribeStateObjects?SentinelName=Consumer&PackageName=Demo&AccessKey=MaCleDeTest123&sentinel=MON-PC&package=HWMonitor&name=/intelcpu/load/0
            //GET
            try
            {
                string param = $"sentinel={sentinel}&package={package}&name={name}&type={type}";
                if (subscriptionId != "")
                    param += $"&subscriptionId={subscriptionId}";
                var ret = RunRequest("SubscribeToStateObjects", param);
                if (ret == null)
                    return;
                if ((ret != "") && (ret.Length > 2))
                {
                    if (subscriptionId == "")
                        subscriptionId = ret.Substring(1, ret.Length - 2);
                    //check if we have an existing request for same element already
                    if (!PropertyToRaise.Exists(m => (m.StateObjectsToCheck.Sentinel == sentinel) && (m.StateObjectsToCheck.Name == name) && (m.StateObjectsToCheck.Package == package) && (m.StateObjectsToCheck.Type == type)))
                    {
                        var attr = new StateObjectLinkAttribute(sentinel, package, name, type);
                        attr.RequestValueOnInit = requestValueOnInit;
                        PropertyToRaise.Add(new PropertyRaiseEvent(attr, null, null, callBack));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }

        static private bool CheckRegisteredStates()
        {
            if (subscriptionId == "")
                return false;

            // http://localhost:8088/rest/constellation/GetStateObjects?SentinelName=Consumer&PackageName=Demo&AccessKey=MaCleDeTest123&subscriptionId=<subId>
            try
            {
                string param = $"subscriptionId={subscriptionId}";
                var ret = RunRequest("GetStateObjects", param);
                if (ret == null)
                    return false;
                var jstate = JsonConvert.DeserializeObject<List<StateObjectFromGet>>(ret);
                List<StateObject> stateObject = new List<StateObject>();
                foreach (var js in jstate)
                    stateObject.Add(js.StateObject);
                if (stateObject != null)
                    if (stateObject.Count > 0)
                        CheckStateObjectsAndNotify(stateObject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
                return false;
            }
            return true;
        }

        //
        // Summary:
        //     Registers all properties flagged with StateObjectLink attribute on a specified
        //     class.
        //
        // Parameters:
        //   type:
        //     The type of the class to register.
        //
        //   instance:
        //     The object whose contains StateObjectLinks (leave 'null' for static instance).
        public static void RegisterStateObjectLinks(Type type, object instance = null)
        {
            bool foundstate = false;
            foreach (var att in type.GetProperties())
            {
                foreach (var det in att.GetCustomAttributes(true))
                {
                    if (det.GetType() == typeof(StateObjectLinkAttribute))
                    {
                        var methode = att.GetSetMethod();
                        StateObjectNotifier obj = new StateObjectNotifier();
                        methode.Invoke(instance, new object[] { obj });
                        var objcall = (StateObjectLinkAttribute)det;
                        PropertyToRaise.Add(new PropertyRaiseEvent(objcall, obj, att, null));
                        //CheckStateObjectsAndNotify
                        RegisterStateObjectCallback(CheckStateObjectsAndNotify, objcall.Sentinel, objcall.Package, objcall.Name, objcall.Type, objcall.RequestValueOnInit);
                        foundstate = true;
                    }
                }
            }
            if (foundstate)
            {
                timerCheckState = new Timer(GetStates);
                timerCheckState.Change(RetryConnectionInterval, Timeout.Infinite);
            }
        }
        //
        // Summary:
        //     Registers all properties flagged with StateObjectLink attribute on a specified
        //     class.
        //
        // Parameters:
        //   instance:
        //     The object whose contains StateObjectLinks.
        public static void RegisterStateObjectLinks(object instance)
        {
            RegisterStateObjectLinks(instance.GetType(), instance);
        }
        //
        // Summary:
        //     Registers all properties flagged with StateObjectLink attribute on a specified
        //     class.
        //
        // Parameters:
        //   instance:
        //     The object whose contains StateObjectLinks (leave 'null' for static instance).
        //
        // Type parameters:
        //   T:
        //     The type of the class to register.
        public static void RegisterStateObjectLinks<T>(object instance = null) where T : class
        {
            RegisterStateObjectLinks(typeof(T), instance);
        }

        #endregion

        private static PackageDescriptor.MemberDescriptor GetMemberDescriptor(PackageDescriptor.MemberDescriptor.MemberType memberType, string memberName, Type parameterType, List<PackageDescriptor.TypeDescriptor> knowTypes)
        {
            if (knowTypes != null)
            {
                DescribeType(parameterType, knowTypes);
            }
            return new PackageDescriptor.MemberDescriptor
            {
                Name = memberName,
                TypeName = parameterType.FullName,
                Type = memberType
            };
        }


        private static string DescribeType(Type type, List<PackageDescriptor.TypeDescriptor> knowTypes)
        {
            if ((!type.IsPrimitive && (type != typeof(object))) && ((type != typeof(string)) && !knowTypes.Any<PackageDescriptor.TypeDescriptor>(t => (t.TypeFullname == type.FullName))))
            {
                PackageDescriptor.TypeDescriptor descriptor = new PackageDescriptor.TypeDescriptor
                {
                    IsArray = type.IsArray,
                    IsEnum = type.IsEnum,
                    IsGeneric = type.IsGenericType,
                    TypeName = type.Name,
                    TypeFullname = type.FullName
                };
                XmlDocumentationReader.AddXmlComment(type, descriptor);
                knowTypes.Add(descriptor);
                if (descriptor.IsEnum)
                {
                    descriptor.EnumValues = new List<PackageDescriptor.MemberDescriptor>();
                    foreach (string str in type.GetEnumNames())
                    {
                        PackageDescriptor.MemberDescriptor descriptor2 = GetMemberDescriptor(PackageDescriptor.MemberDescriptor.MemberType.EnumValue, str, type, null);
                        XmlDocumentationReader.AddEnumXmlComment(str, type, descriptor2);
                        descriptor.EnumValues.Add(descriptor2);
                    }
                }
                else if (descriptor.IsArray)
                {
                    List<string> list1 = new List<string> {
                DescribeType(type.GetElementType(), knowTypes)
            };
                    descriptor.GenericParameters = list1;
                }
                else if (descriptor.IsGeneric)
                {
                    descriptor.GenericParameters = (from t in type.GetGenericArguments() select DescribeType(t, knowTypes)).ToList<string>();
                }
                else
                {
                    descriptor.Properties = new List<PackageDescriptor.MemberDescriptor>();
                    foreach (PropertyInfo info in type.GetProperties())
                    {
                        if (GetCustomAttribute<JsonIgnoreAttribute>(info, false) == null)
                        {
                            PackageDescriptor.MemberDescriptor descriptor3 = GetMemberDescriptor(PackageDescriptor.MemberDescriptor.MemberType.Property, info.Name, info.PropertyType, knowTypes);
                            XmlDocumentationReader.AddXmlComment(info, descriptor3);
                            descriptor.Properties.Add(descriptor3);
                        }
                    }
                }
            }
            return type.FullName;
        }


        public static void DescribeStateObjectTypesFromAssembly(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }
            foreach (Type type in assembly.GetTypes())
            {
                if (GetCustomAttribute<StateObjectAttribute>(type, false) != null)
                {
                    DescribeType(type, PackageDescriptor.StateObjectTypes);
                }
            }
        }

        public static void DescribeStateObjectTypes(params Type[] stateObjectTypes)
        {
            Type[] typeArray = stateObjectTypes;
            for (int i = 0; i < typeArray.Length; i++)
            {
                DescribeType(typeArray[i], PackageDescriptor.StateObjectTypes);
            }
        }





        #endregion

        #region Start and stop

        //
        // Summary:
        //     Shutdowns the current package
        public static void Shutdown()
        {
            if (IsRunning)
            {
                bIsAllInitialzed = false;
                SettingsUpdated = null;
                StateObjectUpdated = null;
                LastStateObjectsReceived = null;
                PackageDescriptor = null;
                if (Package != null)
                {
                    Package.OnShutdown();
                    Package = null;
                }
            }
        }
        //
        // Summary:
        //     Starts the Constellation's package.
        //
        // Parameters:
        //   packageType:
        //     Type of the package.
        //
        //   args:
        //     The arguments.
        //
        // Exceptions:
        //   T:System.Exception:
        //     The package is already started !
        //[DebuggerNonUserCode]
        public static void Start(Type packageType, string[] args = null)
        {
            // Set the Package
            PackageHost.packageType = packageType;
            Package = Activator.CreateInstance(packageType) as IPackage;
            // Create a PackageDescriptor
            PackageDescriptor = new PackageDescriptor();
            PackageDescriptor.PackageName = PackageName;

            //Get all the StateObject types from the assembly
            DescribeStateObjectTypesFromAssembly(null);
            StateObjectKnownTypesAttribute customAttribute = GetCustomAttribute<StateObjectKnownTypesAttribute>(PackageHost.packageType, false);
            if ((customAttribute != null) && (customAttribute.StateObjectKnownTypes.Length != 0))
            {
                DescribeStateObjectTypes(customAttribute.StateObjectKnownTypes);
            }

            // Find all the state objects subscriptions
            RegisterStateObjectLinks(packageType);
            // Register the Message callback as well if we have MessageCallback
            RegisterMessagesObjectLinks(packageType);

            //Register the package
            DeclarePackageDescriptor();
            bIsAllInitialzed = true;

            Package.OnStart();
            //if ((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX))
            //{
            Console.WriteLine("Package started");
            Thread.Sleep(-1);
            //}
            //else
            //{
            //    Console.WriteLine("Press enter to exit");
            //    Console.Read();
            //}
            Shutdown();
        }

        //
        // Summary:
        //     Starts the Constellation's package.
        //
        // Parameters:
        //   args:
        //     The arguments.
        //
        // Type parameters:
        //   TPackage:
        //     The type of the package start class.
        //
        // Exceptions:
        //   T:System.Exception:
        //     The package is already started !
        //[DebuggerNonUserCode]
        public static void Start<TPackage>(string[] args = null) where TPackage : IPackage, new()
        {
            Start(typeof(TPackage), args);
        }

        #endregion

        #region Messages

        static private string messageSubscriptionId = "";

        //
        // Summary:
        //     Registers a callback on message received.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   onData:
        //     The on data.
        public static void RegisterMessageCallback(string key, Action<dynamic> onData)
        {
            // http://localhost:8088/rest/constellation/SubscribeToMessage?SentinelName=Consumer&PackageName=Demo&AccessKey=MaCleDeTest123
            //GET 
            try
            {
                if (messageSubscriptionId == "")
                {
                    var ret = RunRequest("SubscribeToMessage");
                    if (ret == null)
                        return;
                    if ((ret != "") && (ret.Length > 2))
                    {
                        if (messageSubscriptionId == "")
                            messageSubscriptionId = ret.Substring(1, ret.Length - 2);
                    }
                }
                if (onData != null)
                    MessageCallback.Add(key, onData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }
        //
        // Summary:
        //     Registers all methods flagged with MesageCallback attribute on a specified class.
        //
        // Parameters:
        //   source:
        //     The class source.
        public static void RegisterMessageCallbacks(object source)
        {
            RegisterMessagesObjectLinks(source.GetType());
        }

        //
        // Summary:
        //     Registers a callback on message received.
        //
        // Parameters:
        //   key:
        //     The key of the message.
        //
        //   onData:
        //     The callback.
        //
        // Type parameters:
        //   TMessage:
        //     The type of the message.
        public static void RegisterMessageCallback<TMessage>(string key, Action<TMessage> onData)
        {
            RegisterMessageCallback(key, onData);
        }


        //
        // Summary:
        //     Subscribes to the messages of a specified group.
        //
        // Parameters:
        //   groupName:
        //     Name of the group.
        public static void SubscribeMessages(string groupName)
        {
            //http://localhost:8088/rest/constellation/SubscribeToMessageGroup?SentinelName=Consumer&PackageName=Demo&AccessKey=MaCleDeTest123&subscriptionId=xxxxx&group=A
            try
            {
                string param = $"group={groupName}";
                if (messageSubscriptionId == "")
                {
                    var ret = RunRequest("SubscribeToMessageGroup", param);
                    if ((ret != "") && (ret.Length > 2))
                    {
                        if (messageSubscriptionId == "")
                            messageSubscriptionId = ret.Substring(1, ret.Length - 2);
                    }
                }
                else
                {
                    param += $"&subscriptionId={messageSubscriptionId}";
                    RunRequest("SubscribeToMessageGroup", param);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }

        static private Dictionary<MessageCallbackAttribute, MethodInfo> MessageToRaise = new Dictionary<MessageCallbackAttribute, MethodInfo>();
        static private Dictionary<string, Action<dynamic>> MessageCallback = new Dictionary<string, Action<dynamic>>();

        private static void RegisterMessagesObjectLinks(Type packageType)
        {
            bool foundmess = false;
            foreach (var att in packageType.GetMethods())
            {
                foreach (var det in att.GetCustomAttributes(true))
                {
                    if (det.GetType() == typeof(MessageCallbackAttribute))
                    {
                        //var methode = att.GetSetMethod();

                        var objcall = (MessageCallbackAttribute)det;
                        if (objcall.Key == null)
                            objcall.Key = att.Name;
                        MessageToRaise.Add(objcall, att);

                        //var deleg = Delegate.CreateDelegate(att.GetType(), att);
                        RegisterMessageCallback(objcall.Key, null);
                        if (!objcall.IsHidden)
                        {
                            PackageDescriptor.MessageCallbackDescriptor descriptor = new PackageDescriptor.MessageCallbackDescriptor
                            {
                                MessageKey = objcall.Key,
                                Description = (objcall.Description != null) ? objcall.Description : string.Empty,
                                Parameters = GetParametersDescription(att.GetParameters(), PackageDescriptor.MessageCallbackTypes)
                            };
                            XmlDocumentationReader.AddXmlComment(att, descriptor);
                            if (objcall.ResponseType != null)
                            {
                                descriptor.ResponseType = DescribeType(objcall.ResponseType, PackageDescriptor.MessageCallbackTypes);
                            }
                            else if (att.ReturnType != typeof(void))
                            {
                                descriptor.ResponseType = DescribeType(att.ReturnType, PackageDescriptor.MessageCallbackTypes);
                            }
                            PackageDescriptor.MessageCallbacks.Add(descriptor);
                        }
                        foundmess = true;
                    }
                }
            }
            if (foundmess)
            {
                timerMessage = new Timer(GetMessages);
                timerMessage.Change(RetryConnectionInterval, Timeout.Infinite);
            }
        }

        private static List<PackageDescriptor.MemberDescriptor> GetParametersDescription(ParameterInfo[] parametersInfo, List<PackageDescriptor.TypeDescriptor> knowTypes)
        {
            List<Constellation.PackageDescriptor.MemberDescriptor> list = new List<PackageDescriptor.MemberDescriptor>();
            if (parametersInfo != null)
            {
                foreach (ParameterInfo info in parametersInfo)
                {
                    Type parameterType = info.ParameterType;
                    if (parameterType != null)
                    {
                        list.Add(GetMemberDescriptor(PackageDescriptor.MemberDescriptor.MemberType.MethodParameter, info.Name, parameterType, knowTypes));
                    }
                }
            }
            return list;
        }
        //
        // Summary:
        //     Sends the message to the Constellation hub.
        //
        // Parameters:
        //   scope:
        //     The scope.
        //
        //   key:
        //     The key of the message.
        //
        //   value:
        //     The message data.
        public static void SendMessage(MessageScope scope, string key, object value)
        {
            //SendMessage
            // {
            //  "Scope" : { "Scope" : "<type>", Args: [ "<arg1>", "<args2>", .... ], "SagaId":"Identifiant de la Saga" },
            //  "Key" : "<Key>",
            //  "Data" : "<Data>"
            //}
            try
            {
                PostScope param = new PostScope();
                param.Scope = scope;
                param.Key = key;
                param.Data = value;
                RunRequest("SendMessage", JsonConvert.SerializeObject(param), false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
            }
        }


        //
        // Summary:
        //     Creates a MessageScope and return the proxy to send message
        //
        // Parameters:
        //   package:
        //     The package.
        //
        // Returns:
        //     The dynamic proxy.
        //[return: Dynamic]
        public static dynamic CreateMessageProxy(string package)
        {
            //TODO : check it is possible with this 
            return null;
        }
        //
        // Summary:
        //     Creates a MessageScope and return the proxy to send message
        //
        // Parameters:
        //   type:
        //     The scope type.
        //
        //   args:
        //     The scope arguments.
        //
        // Returns:
        //     The dynamic proxy.
        //[return: Dynamic]
        public static dynamic CreateMessageProxy(MessageScope.ScopeType type, params string[] args)
        {
            //TODO: check it is possible with this
            return null;
        }

        #endregion

        #region log
        public static void WriteDebug(string format, params object[] args)
        {
            Debug.WriteLine(string.Format(format, args));
        }

        public static void WriteError(string format, params object[] args)
        {
            WriteLog(LogLevel.Error, string.Format(format, args));
        }
        public static void WriteInfo(string format, params object[] args)
        {
            WriteLog(LogLevel.Info, string.Format(format, args));
        }
        public static void WriteLog(LogLevel level, string message, bool await = false, bool catchException = false)
        {
            //http://localhost:8088/rest/constellation/WriteLog?SentinelName=MyVirtualSentinel&PackageName=MyVirtualPackage&AccessKey=MyAccessKey&message=Hello World
            //http://localhost:8088/rest/constellation/PurgeStateObjects?SentinelName=MyVirtualSentinel&PackageName=MyVirtualPackage&AccessKey=MyAccessKey&message=Il y  a une erreur ici&level=Error
            if (level == LogLevel.Debug)
            {
                Debug.WriteLine(message);
            }
            else
                try
                {
                    RunRequest("WriteLog", $"message={message}&level={level.ToString()}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UPS: {ex.Message}");
                }

        }
        public static void WriteWarn(string format, params object[] args)
        {
            WriteLog(LogLevel.Warn, string.Format(format, args));
        }
        #endregion

        #region internal

        static private Timer timerMessage;
        static private Timer timerCheckState;
        static private bool bIsCheckingState = false;

        private static void CheckRetryMessages()
        {
            RetryMessages++;
            // we most likely had an issue, so wait 3 seconds and retry
            if (RetryMessages < RetryMaxError)
            {
                timerMessage = new Timer(GetMessages);
                timerMessage.Change(RetryConnectionInterval, Timeout.Infinite);
            }
            else
            {
                messageSubscriptionId = "";
                //looks like we may have lost connection with the sentinel for 3 minutes = 3s x 60 times
                foreach (var prop in MessageToRaise)
                {
                    RegisterMessageCallback(prop.Key.Key, null);
                }
            }
        }


        // Used to Get the messages
        private static void GetMessages(object state)
        {
            //http://localhost:8088/rest/constellation/GetMessages?SentinelName=Consumer&PackageName=Demo&AccessKey=MaCleDeTest123&subscriptionId=xxxxx            
            try
            {
                if (timerMessage != null)
                    timerMessage.Dispose();

                string param = $"subscriptionId={messageSubscriptionId}";
                var ret = RunRequest("GetMessages", param);
                if (ret == null)
                    CheckRetryMessages();
                if (ret.Length >= 2)
                {
                    //The "Data" returned is sometimes an array, sometimes 
                    var Messages = JsonConvert.DeserializeObject<MessageScopeResponse[]>(ret);
                    foreach (var msg in Messages)
                    {
                        //Check first if we have a property matching
                        foreach (var prop in MessageToRaise)
                        {
                            if (prop.Key.Key == msg.Key)
                            {
                                var args = PrepDataFromGetMessage(msg.Data, prop.Value.GetParameters());
                                var method = prop.Value.Invoke(Package, args);
                                if ((method != null) || (msg.Scope.SagaId != null))
                                {
                                    //TODO: make nsure response is working.
                                    //TODO: make the Saga working
                                    MessageContext context = new MessageContext();
                                    context.Scope = new MessageScope((MessageScope.ScopeType)msg.Scope.scope, new string[] { method?.ToString() });
                                    context.Sender = new MessageSender();
                                    context.Sender.ConnectionId = msg.Sender.ConnectionId;
                                    context.Sender.FriendlyName = PackageHost.SentinelName; // msg.Sender.FriendlyName;
                                    context.Sender.Type = MessageSender.SenderType.ConsumerHttp;

                                    PackageHost.SendMessage(context.Scope, "__Response", context.Sender);

                                }

                            }
                        }
                        //Check if we have a registered Method
                        foreach (var prop in MessageCallback)
                        {
                            if (prop.Key == msg.Key)
                            {
                                var args = PrepDataFromGetMessage(msg.Data, prop.Value.GetMethodInfo().GetParameters());
                                prop.Value.Invoke(args);
                            }
                        }
                    }
                }
                RetryMessages = 0;
                //call back the GetMessage()
                GetMessages(null);


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS: {ex.Message}");
                //Wait 3 seconds and retry
                CheckRetryMessages();
            }
        }

        static private object[] PrepDataFromGetMessage(object data, ParameterInfo[] parameters)
        {
            List<object> args = new List<object>();
            if (parameters.Length == 0)
                return null;
            else if (parameters.Length == 1)
            {
                try
                {
                    var arg = Convert.ChangeType(((JObject)data).ToObject(parameters[0].ParameterType), parameters[0].ParameterType);
                    args.Add(arg);
                }
                catch (Exception)
                {
                    var arg = Convert.ChangeType(data, parameters[0].ParameterType);
                    args.Add(arg);
                }
                return args.ToArray();
            }
            else
            {
                for (int i = 0; i < parameters.Length; i++)
                {

                    var arr = (JArray)(data);
                    try
                    {
                        var arg = Convert.ChangeType(((JObject)arr[i]).ToObject(parameters[i].ParameterType), parameters[i].ParameterType);
                        args.Add(arg);
                    }
                    catch (Exception)
                    {
                        var arg = Convert.ChangeType(arr[i].ToString(), parameters[i].ParameterType);
                        args.Add(arg);
                    }
                }
                return args.ToArray();
            }
        }

        private static void CheckRetryStates()
        {
            RetryStateObjects++;
            // we most likely had an issue, so wait 3 seconds and retry
            if (RetryStateObjects < RetryMaxError)
            {
                timerCheckState = new Timer(GetStates);
                timerCheckState.Change(RetryConnectionInterval, Timeout.Infinite);
            }
            else
            {
                //looks like we may have lost connection with the sentinel for 3 minutes = 3s x 60 times
                foreach (var prop in PropertyToRaise)
                {
                    //Let's reinitialize the request process
                    subscriptionId = "";
                    //CheckStateObjectsAndNotify
                    RegisterStateObjectCallback(CheckStateObjectsAndNotify, prop.StateObjectsToCheck.Sentinel, prop.StateObjectsToCheck.Package, prop.StateObjectsToCheck.Name, prop.StateObjectsToCheck.Type, prop.StateObjectsToCheck.RequestValueOnInit);
                }
            }

        }

        private static void GetStates(object state)
        {
            try
            {
                if (timerCheckState != null)
                    timerCheckState.Dispose();
                if ((!bIsCheckingState) && (bIsAllInitialzed))
                {
                    var ret = CheckRegisteredStates();
                    if (ret)
                    {
                        GetStates(null);
                        RetryStateObjects = 0;
                    }
                    else
                    {
                        CheckRetryStates();
                    }
                }
                else
                {
                    CheckRetryStates();
                }
            }
            catch (Exception ex)
            {
                bIsCheckingState = false;
                Debug.WriteLine($"UPS: {ex.Message}");
                CheckRetryStates();
            }
        }

        private static string RunRequest(string commande, string param = "", bool isGet = true)
        {
            string URL = BuildURL() + commande;
            if (param != "")
            {
                if (isGet)
                    URL += "?" + param;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("SentinelName", SentinelName);
            request.Headers.Add("PackageName", PackageName);
            request.Headers.Add("AccessKey", AccessKey);

            if (isGet)
            {
                request.Method = "GET";
            }
            else
            {
                request.ContentType = "application/json";
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(param);
                request.ContentLength = byteArray.Length;
                var datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UPS command{commande}: {ex.Message}");
            }
            return null;
        }

        private static string BuildURL()
        {
            return ((isHttps) ? "https://" : "http://") + ServerName + ":" + Port + "/rest/constellation/";
        }

        public static TAttribute GetCustomAttribute<TAttribute>(MemberInfo member, bool inherit = false) where TAttribute : Attribute =>
    member.GetCustomAttribute<TAttribute>();





        #endregion
    }
}
