## Casing Conventions: A Detailed Guide

In our project, `PascalCase` and `camelCase` are not just stylistic choices; they are signals that instantly tell a developer what kind of "thing" they are looking at.

### 1. PascalCase (also known as UpperCamelCase)

**The Rule:** The first letter of the word is capitalized. The first letter of every subsequent joined word is also capitalized.

-   `Item`
-   `ItemData`
-   `PlayerInventory`
-   `StartMinigame`

**The Meaning: "This is a Blueprint, a Concept, or an Action."**

When you see `PascalCase`, your brain should immediately think: "This is a **definition** or a **command**." It's the original blueprint for something, not a specific instance of it.

#### Where We Use PascalCase:

**A) Class and Struct Names:** A class is the fundamental blueprint for an object.
```csharp
// This is the DEFINITION of what an ItemData is.
public class ItemData : ScriptableObject
{
    // ...
}
```
**B) Enum Names:** An enum is a blueprint for a set of named constants.
```csharp
// This is the DEFINITION of the possible rarities.
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
```
**C) Method (Function) Names:** A method is a blueprint for an action that an object can perform. It's a defined capability.
```csharp
// This is the DEFINITION of the "Craft" action.
public void CraftItem()
{
    // ...
}
```

---

### 2. camelCase (also known as lowerCamelCase)

**The Rule:** The first letter of the word is **lowercase**. The first letter of every subsequent joined word is capitalized.

-   `item`
-   `itemData`
-   `playerInventory`
-   `baseValue`

**The Meaning: "This is a Specific Instance or a Piece of Data."**

When you see `camelCase`, your brain should immediately think: "This is a **container for something**." It's a variable, a specific object, a piece of data that lives in memory. It's the house built *from* the blueprint, not the blueprint itself.

#### Where We Use camelCase:

**A) Variable / Field Names:** These are the containers that hold the data inside your classes.
```csharp
public class PlayerInventory : MonoBehaviour
{
    // This is a specific list of items that belongs to THIS player.
    public List<ItemData> items;

    // This is a specific number representing the player's gold.
    public int goldAmount;
}
```
**B) Method Parameters:** These are temporary containers that hold the data you pass *into* a method.
```csharp
// 'itemToSell' is a temporary variable that exists only inside this method.
public void SellItem(ItemData itemToSell)
{
    // ...
}
```
**C) Private Variables (with `_` prefix):** This is a special subset of camelCase we use. The underscore `_` is an additional signal that means "This variable is private and should only be used inside this class."
```csharp
public class ForgeMinigame : MonoBehaviour
{
    // The underscore signals that this is an internal, private variable.
    private float _currentHeat;
}
```

---

### The Analogy: The Car Blueprint vs. Your Car

This is the best way to understand the difference:

-   The **Blueprint** for a car model is called `FordMustang`. This is a **Class**, so it's **`PascalCase`**. It defines what a Mustang is, its horsepower, its shape, etc.
-   The **specific car** sitting in your driveway is `myRedMustang`. This is a **variable**, an **instance** of the blueprint, so it's **`camelCase`**.
-   The **color** of your car is a property of that instance. It's a piece of data called `color`, so it's **`camelCase`**.
-   An **action** that all Mustangs can perform is defined on the blueprint. It's called `StartEngine()`. This is a **Method**, so it's **`PascalCase()`**.

Putting it together in code:
```csharp
// The Blueprint (Class in PascalCase)
public class FordMustang
{
    // The Data Container (variable in camelCase)
    public string color;

    // The Action Blueprint (Method in PascalCase)
    public void StartEngine()
    {
        Debug.Log("Engine started!");
    }
}

// In another script, we create a specific instance.
public class MyGarage : MonoBehaviour
{
    // A variable to hold our specific car (instance in camelCase)
    public FordMustang myRedMustang;

    void Start()
    {
        // We create a new instance of the blueprint
        myRedMustang = new FordMustang();

        // We set the data for our specific instance
        myRedMustang.color = "Red";

        // We tell our specific instance to perform the action
        myRedMustang.StartEngine();
    }
}
```

By sticking to this standard, we make our code predictable and easy to read, which is absolutely essential for collaborating and building this game on schedule.
