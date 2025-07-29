# Instructions to Push readarrv2 Branch

Since cursor[bot] cannot accept GitHub invitations, you'll need to push the changes manually. I've prepared patch files with all the commits.

## Option 1: Apply Patches (Recommended)

1. Clone your fork locally:
```bash
git clone https://github.com/Lasidar/Readarrv2_test.git
cd Readarrv2_test
```

2. Create and checkout the readarrv2 branch:
```bash
git checkout -b readarrv2
```

3. Apply the patches (download the `/workspace/readarrv2-patches/` directory first):
```bash
git am 0001-Initial-Readarr-v2-transformation-setup.patch
git am 0002-Phase-1-Complete-Core-transformation-from-Sonarr-to-.patch
git am 0003-Phase-2.1-Complete-Domain-model-transformation-n-n-T.patch
```

4. Push to your fork:
```bash
git push -u origin readarrv2
```

## Option 2: Direct Push from This Environment

If you can provide a Personal Access Token with repo permissions:

1. Create a token at: https://github.com/settings/tokens
2. Run in this terminal:
```bash
cd /workspace/readarrv2
git remote set-url origin https://YOUR_GITHUB_USERNAME:YOUR_TOKEN@github.com/Lasidar/Readarrv2_test.git
git push -u origin readarrv2
```

## What's Being Pushed

The readarrv2 branch contains 3 commits with the complete transformation work:
- Initial Readarr v2 transformation setup
- Phase 1: Core transformation from Sonarr to Readarr  
- Phase 2.1: Domain model transformation (Series→Author, Season→Book, Episode→Edition)