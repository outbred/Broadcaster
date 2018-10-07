# Broadcaster
A lightweight, simple, and efficient system to replace clunky, memory-leak inducing, typical event subscriptions in .NET Standard. 

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

### Show me some :heart: and star this repo to support my project

# Pull Requests

I welcome and encourage all pull requests. It usually will take me within 24-72 hours to respond to any issue or request. Here are some basic rules to follow to ensure timely addition of your request:

1.  Match coding style (braces, spacing, etc.), or I will do it for you
2.  If its a feature, bugfix, or anything please only change code to what you specify.
3.  Please keep PR titles easy to read and descriptive of changes, this will make them easier to merge :)
4.  Pull requests _must_ be made against `develop` branch. Any other branch (unless specified by the maintainers) will get rejected.
5.  Check for existing [issues](https://github.com/outbred/oops/issues) first, before filing an issue.
6.  Make sure you follow the set standard as all other projects in this repo do

### Created & Maintained By

[Brent Bulla](https://github.com/outbred) ([@outbred](https://www.twitter.com/outbred))
([Insta](https://www.instagram.com/outbred))

> If you found this project helpful or you learned something from the source code and want to thank me, consider buying me a cup of  <img src="https://vignette.wikia.nocookie.net/logopedia/images/a/ad/Dr._Pepper_1958.jpg/revision/latest?cb=20100924201743" height="25em" />  -  [PayPal](https://paypal.me/brentbulla/)

# License
MIT License

Copyright (c) 2018 Brent Bulla

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
