# Broadcaster
A system to replace clunky, memory-leak inducing, typical event subscriptions in .NET Standard. 

Typcial event subscriptions like this:

```C#
SomeObservableCollection.CollectionChanged += (s,e) => Ahandler();
```

...will _always_ result in a memory leak.  This forces the conscientious developer to create a method like this:

```C#
SomeObservableCollection.CollectionChanged += OnCollectionChanged;
...
private void OnCollectionChanged(object sender, CollectionChangedEventArgs a) 
{
... do some stuff ...
}
```

And then when the subscription is no longer desired:

```C#
SomeObservableCollection.CollectionChanged -= OnCollectionChanged;
```

If used correctly, this will never create a memory leak (emphasis on '*if*').  This mechanism is clunky, wordy, and obtuse.

UI still and probably will always require true events (marked with the *event* keyword) to latch onto, but for the rest of the business code,
I think we'd all prefer something a bit...cleaner and easier to use.

*Broadcaster* provides a frameworkd for easily listening for, handling, and unsubscribing from events _that allows the use of lambdas (!!)_.  
It's a thing of beauty to use in a large application (which I have) and once this easy to use system is understood, it is simple to 
unsubscribe to any and all events.  If you are familiar with the Reactive framework, then this will be right up your alley.

The syntax is similar to subscribing to a Reactive event, but less wordy and obtuse, but it does follow the same disposable token
methodology (because that is far and above the best way to subscribe/unsubscribe from events).

## _If you are already using the Reactive framework, just use their event pattern.  This is a lightweight framework that just focuses on simple event subscription/unsubscription and nothing else._

If you're using the EventAggregator or some other PubSub mechanism, this may be a better alternative b/c it is strongly typed when it can be, but allows loose typing with enums, too.  And it awaits all handlers!  Can be used in a global scope, or a local scope.
