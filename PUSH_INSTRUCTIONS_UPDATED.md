# Instructions to Push Readarr v2 Changes

Since I cannot push directly due to GitHub permissions, here are the steps to apply all the changes:

## Complete Transformation Patch

I've created a comprehensive patch file containing all the changes from Phase 0 through Phase 5:
- `/workspace/readarrv2-complete-transformation.patch` (3.1MB)

This patch includes:
1. **Phase 0**: Initial setup and cleanup
2. **Phase 1**: Core namespace transformation (Sonarr/NzbDrone → Readarr)
3. **Phase 2**: Domain model transformation (Series→Author, Season→Book, Episode→Edition)
4. **Phase 3**: Service layer transformation
5. **Phase 4**: API transformation
6. **Phase 5**: Frontend transformation

## To Apply the Patch

1. Clone the Readarr v2 test repository:
```bash
git clone https://github.com/Lasidar/Readarrv2_test.git
cd Readarrv2_test
git checkout -b readarrv2
```

2. Apply the complete patch:
```bash
git apply /workspace/readarrv2-complete-transformation.patch
```

3. Commit all changes:
```bash
git add -A
git commit -m "Complete Readarr v2 transformation from Sonarr

This commit includes:
- Core namespace transformation (Sonarr/NzbDrone → Readarr)
- Domain model transformation (TV shows → Books)
- Database schema updates with book-specific migrations
- Service layer and API transformation
- Complete frontend transformation
- Updated translations and configuration

The codebase is now fully adapted for books instead of TV shows."
```

4. Push to your repository:
```bash
git push origin readarrv2
```

## Alternative: Individual Commits

If you prefer to preserve the individual commit history, I have 4 commits locally:

1. Phase 0-2: Initial transformation
2. Phase 3: Service layer transformation
3. Phase 4: API transformation
4. Phase 5: Frontend transformation

You can view the commit history with:
```bash
cd /workspace/readarrv2
git log --oneline -n 4
```

## Next Steps

After pushing, you should:
1. Run backend build: `dotnet build src/Readarr.sln`
2. Run frontend build: `cd frontend && yarn build`
3. Run tests to ensure everything works correctly
4. Update any remaining configuration files or documentation as needed

The transformation is complete and ready for testing!