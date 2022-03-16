Test site hosted on [Heroku](https://spirit-island-randomiser.herokuapp.com/)

# About

This is a small web app written using c# and [Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-6.0).
It allows you to easily generate random set-ups for the board game [Spirit Island](https://www.boardgamegeek.com/boardgame/162886/spirit-island) within specific parameters. 

The design was heavily inspired by the excellent [Spirit Islander](https://www.spiritislander.com/).

The purpose of this app is to experiment with Blazor in a 'real world' scenario and implement features that are missing from Spirit Islander, such as aspects and supporting adversaries.

# Todo

- Use incognito mode to verify default settings.
- Refactor to use 'IsValid' logic on GameSetup for thematic map as well. This will remove a lot of code and remove need to try and calculate number of board combinations.
- On results page, add setup instructions for extra boards, adversaries, etc. Also add pictures for map setups.
- Add support for custom spirits & adversaries
- Add tests and ensure invalid edge cases are handled correctly (e.g. 'no adversary' is not selected, but secondary adversary is 'blocked')
- improve responsive design to support more columns on wider displays (e.g. 4 columns of adversaries on desktop monitors)
- Bug - Select all expansions. Select lightning - all aspects are selected. Deselect 'Base'. Deselect 'Promo 2' and 'Jagged Earth' then reselect them. Lightning is still selected, but none of it's aspects are.
