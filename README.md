# CompletedGamesList
A tool to generate a list of completed games for use in Steam descriptions, as seen on my profile: http://steamcommunity.com/id/Rallymen007/profile/

## Files required
Along the .exe file, you will need 2 different files:
-header.txt : contains the fixed header to be added to every generated file
-games.csv : contains a CSV representation of your game list following this format:

Platform,GameName,is100,Comm

An example file is present in the repository.

## NEW in Release 4
Generation of HTML lists, if you're so good you overflow the Steam section size.

## Extra information
Any value in the platform column is a valid platform header, so you can add as many different game platforms as you can.
For 100% completed games to show properly, you'd need the :100percent: Steam emoticon: http://steamcommunity.com/market/listings/753/282800-%3A100percent%3A
You can find a bundled style.css file to use for HTML generated lists.