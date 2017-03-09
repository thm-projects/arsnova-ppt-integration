# Contributing

**ARSnova: powerpoint-edition** needs you! If you are interested in helping, here is a short guide.

## Step-by-step summary

1. First, fork and clone our repository.
2. Create a topic branch.
3. Remember to use test-driven development, it will make your live much easier.
4. Make your changes. Be sure to provide clean commits and to write [expressive commit messages][commit-message].
5. Check your style: All style settings are configured in [this file][settings]. Import them and use ReSharper for "on-the-fly" code checking.
6. Stay up to date with our repository: Rebase to our `staging` branch using `git rebase`.
7. Push the changes to your topic branch.
8. Run all the tests.
9. Finally, [submit a merge request][merge-request].

If you don't feel like writing code, you could also update the documentation. And if you find any bugs, feel free to [open a new issue][new-issue].

[settings]: https://git.thm.de/arsnova/powerpoint-integration/blob/staging/projectSettings.DotSettings
[commit-message]: http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html
[merge-request]: https://git.thm.de/arsnova/powerpoint-integration/merge_requests/new
[new-issue]: https://git.thm.de/arsnova/powerpoint-integration/issues/new

## How we review merge requests

To get your merge request accepted faster, you should follow our review process before submitting your request. Here is our list of dos and don'ts.

### No merge conflicts

This is a no-brainer. Keep your branches up to date so that merges will never end up conflicting. Always test-merge your branches before submitting your pull requests. Ideally, your branches are fast-forwardable, but this is not a requirement.

### Code Style

Please check your code against our code guidelines defined with ReSharper.

### Project structure

Please take a look at our project file structure:

```
Business/
    Business
    Contract
    Model
Common/
    Common
    Contract
    Enum
    Helpers
    Resources
Communication/
    Communication
    Contract
    Model
Installation/
    Setup
Presentation/
    Presentation
    Configuration
Test/
    Business/
    Communication/
```
			
If questions regarding structures, patterns or frameworks occure, contact us or check out the [thesis][] to this project.


### Packages

We are using NuGet as our package manager.

### Summary

It all comes down to

* reviewing your own changes,
* keeping your commits clean and focused,
* and always staying up to date.

If you keep these things in mind, your merge requests will be accepted much faster. Happy coding!
