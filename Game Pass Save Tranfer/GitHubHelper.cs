﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

public class GitHubHelper
{
    #region Constructors
    public GitHubHelper(string repositoryOwner, string repositoryName)
    {
        RepositoryOwner = repositoryOwner;
        RepositoryName = repositoryName;
    }
    #endregion

    #region Variables
    private string RepositoryOwner;
    private string RepositoryName;
    #endregion

    #region Methods
    /// <summary>
    /// Check if a new release is available on GitHub
    /// </summary>
    /// <returns>true a new release is available, else false</returns>
    public async Task<bool> CheckNewerVersion()
    {
        //Get all releases from GitHub
        //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
        GitHubClient client = new GitHubClient(new ProductHeaderValue(RepositoryName));
        IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(RepositoryOwner, RepositoryName);

        if (releases.Count == 0)
            return false;

        //Setup the versions
        Version latestGitHubVersion;
        if (!Version.TryParse(releases[0].TagName, out latestGitHubVersion) &&
            !Version.TryParse(releases[0].Name, out latestGitHubVersion))
            return false;
        Version localVersion = Assembly.GetExecutingAssembly().GetName().Version;
        //Compare the Versions
        //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
        int versionComparison = localVersion.CompareTo(latestGitHubVersion);
        if (versionComparison < 0)
        {
            //The version on GitHub is more up to date than this local release.
            return true;
        }
        else if (versionComparison > 0)
        {
            //This local version is greater than the release version on GitHub.
            return false;
        }
        else
        {
            //This local Version and the Version on GitHub are equal.
            return false;
        }
    }

    /// <summary>
    /// Open a link to the lastest release
    /// </summary>
    public void Update()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = $"https://github.com/{RepositoryOwner}/{RepositoryName}/releases",
            UseShellExecute = true
        };
        Process.Start(psi);
    }
    #endregion
}