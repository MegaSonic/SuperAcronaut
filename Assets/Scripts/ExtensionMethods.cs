using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public static class ExtensionMethods {

    public static T GetInterface<T>(this GameObject inObj) where T : class
    {
        return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
    }

    public static IEnumerable<T> GetInterfaces<T>(this GameObject inObj) where T : class
    {
        return inObj.GetComponents<Component>().OfType<T>();
    }

}
