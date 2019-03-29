# Zenfolio7 - Utility to work with a user's Zenfolio account

Zenfolio7 (just a temporary name, was Zensync back when it was a PHP script) is a tool to manipulate a user's Zenfolio account.  The initial use will be to sync a local root directory with the root of a Zenfolio account.  Follow-on functionality will come from there.  It's written in WPF because SOAP support in UWP isn't easy to implement, and I really didn't want to create a bunch of back-end code to replicate the auto-generated code that Visual Studio when you import Zenfolio's WSDL.

# Current Working Features

1. Log into Zenfolio account, acquire data tree
2. Browse data tree, view small images
3. Browse local directories

# Planned Features

0. Organize the local directory.  Allow the user to take an existing local directory and transform it using rules.  This will likely use Magick.net to read EXIF data.  The goal is to take an assortment of files, and organize them.  Initially it will be by date - in a RootDir/Year/Month format.
1. Synchronize a local directory (i.e.: C:\MyPhotos) to a Zenfolio root directory.  This will be a replication of the ZenSync method - compatible items that are not in the Zenfolio data tree will be pushed up to Zenfolio in a queue.
2. An upload Queue.  :-)
3. Conflict resolution - where items are different, present a list and allow the user to decide whether to do actions (overwrite, skip, etc)
4. Extend the orgainze functionality with customizable rules

I've borrowed code from many places (StackOverflow FTW!), but specifically:
1. Directory Browser heavily leveraged this: https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
2. Zenfolio login and API code from: https://www.zenfolio.com/zf/help/api/examples
