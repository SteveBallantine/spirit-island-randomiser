Test site hosted on [Heroku](https://spirit-island-randomiser.herokuapp.com/)

# About

This is a small web app written using c# and [Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-6.0).
It allows you to easily generate random set-ups for the board game [Spirit Island](https://www.boardgamegeek.com/boardgame/162886/spirit-island) within specific parameters. 

The design was heavily inspired by the excellent [Spirit Islander](https://www.spiritislander.com/).

The purpose of this app is to experiment with Blazor in a 'real world' scenario and implement features that are missing from Spirit Islander, such as aspects and supporting adversaries.

# Todo

- Add tooltips to explain some options (e.g. block/force/allow)
- Add support for aspects
- Add support for custom spirits & adversaries
- Add tests and ensure invalid edge cases are handled correctly (e.g. 'no adversary' is not selected, but secondary adversary is 'blocked')
- Update the 'board combinations' calculation to take imbalanced arcade board pairs into account.

