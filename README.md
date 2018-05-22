# EZApps
a library which introduces some (hopefully useful) dynamic classes.  
The target of this library is to minimize the use of reflection as much as possible

##### please note that this is not yet complete nor it's good for production code, it's just a fun library to play with

##Nuget:
https://www.nuget.org/packages/EZAppz.Core/

## Current classes :

### `DescribableObject`
Basically a dynamic class which has a dictionary as a backing end (instead of plain fields for each property), which helped introduce some methods like `GetPropertyValue(PropertyName)` and `SetPropertyValue(PropertyName,Value)` without any reflection at all
##### Note : this class is used as the base class for all classes in this library

### `DescribableList<T>`
just a wrapper around `List<T>` that implments `DescribableObject`

### `NotifyBase`
a base class for all notification-based classes (can be really useful for wpf applications)
which implments `INotifyPropertyChanged`and `INotifyPropertyChanging` and of course inherits from `DescribableObject`

it also introduces some useful mechanisms like Relations between properties

i.e. when property `A` changes, notify that properties `B,C,D,...` also changed

this can be implmented using `RegisterRelationProperty`, `RegisterChangingAction`, `RegisterChangedAction`, `RegisterChangingAndChangedAction`

### `NotifyValueCollection<T>`

a collection that inherits `NotifyBase` and wraps a `List<T>`, this is basically the equivalent of an `ObservableCollection<T>` but with more customizations

### `NotifyCollection<T>`

a collection that inherits `NotifyValueCollection<T>` but expands it by restricting the items to be notifiable (i.e. implment `INotifyBase`)

this helped introduce more events like `ItemPropertyChanged` and `ItemPropertyChanging` that detect the changes in the elements contained in the list

### `ResettableObject`
An object whose properties can be reset to previous values, it inherits `NotifyBase` and exploits the changing/changed notifications to maintain a list of old values for each property

### `ResettableValueCollection`
the same as `NotifyValueCollection<T>` but with resetting 
