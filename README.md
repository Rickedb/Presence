# Repo containing a graduation project
---

## Description

The main idea of "Presence" is to be a local chat, which there is a Chat server and all the Apps connect to that server and start chatting with each other.
The idea was to be an Unique URL, since someone that hosts it in a local network would have the SAME URL forever (provided by a DNS).

---

### Directories: 

* App - Android app made in Xamarin (Integration Xamarin + Visual Studio 2015);
* Service - Contains the Core library + Web MVC + Windows Service (Which is the chat server).

### Core Library

Contains all entities and library for threading, logging, etc etc. Briefly, contains all Business logic.

### Windows Service

It's the "server side" of the entire project, it contains chat hubs that will be the one to receive messages and distribute them among the chats.

### Web

Web application made in MVC.NET, simple web app that does the same as de Android App

### App

It's an Android App made in Xamarin, it's not completed, it still have some issues but kinda works good, and, of course it needs a better layout.
It uses signalR to comunicate with the server.

## Conclusion

Since it's a graduation project, it wasn't made at it's best, still have some shitty hard codes and things like that (you know, you start to get mad sometimes and then code "without thinking right" because you only want to end it up).
Learned a lot about SignalR, it was a good way to test the library too.

Of course you going to find better things out on github, but maybe, if you want kinda "newbie" application, you can download it or fork or do whatever you want to.

Made yourself at home!
