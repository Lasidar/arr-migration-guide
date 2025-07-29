# UI/Frontend Migration Patterns

## Overview
This document analyzes the frontend architecture evolution from Sonarr through Lidarr to Readarr, focusing on React component patterns, state management, and UI/UX adaptations.

## Frontend Technology Stack

### Common Stack (All Projects)
- **Framework**: React with TypeScript
- **State Management**: Redux with Redux Toolkit
- **Styling**: CSS Modules
- **Build Tool**: Webpack
- **API Communication**: Axios with SignalR for real-time updates

## Component Architecture Evolution

### 1. Component Organization Pattern

**Sonarr Structure:**
```
frontend/src/
├── Series/
│   ├── SeriesIndex.tsx
│   ├── SeriesDetails.tsx
│   └── SeriesCard.tsx
├── Episode/
│   ├── EpisodeRow.tsx
│   └── EpisodeDetails.tsx
└── Components/
    └── (shared components)
```

**Lidarr Structure:**
```
frontend/src/
├── Artist/
│   ├── ArtistIndex.tsx
│   ├── ArtistDetails.tsx
│   └── ArtistCard.tsx
├── Album/
│   ├── AlbumRow.tsx
│   └── AlbumDetails.tsx
└── Components/
    └── (shared components)
```

**Readarr Structure:**
```
frontend/src/
├── Author/
│   ├── AuthorIndex.tsx
│   ├── AuthorDetails.tsx
│   └── AuthorCard.tsx
├── Book/
│   ├── BookRow.tsx
│   └── BookDetails.tsx
└── Components/
    └── (shared components)
```

### 2. Component Reusability Pattern

**Base Component Example:**
```typescript
// Components/MediaIndex/MediaIndex.tsx
interface MediaIndexProps<T> {
  items: T[];
  columns: Column[];
  onItemSelect: (item: T) => void;
}

function MediaIndex<T>({ items, columns, onItemSelect }: MediaIndexProps<T>) {
  return (
    <Table>
      <TableHeader columns={columns} />
      <TableBody>
        {items.map(item => (
          <TableRow key={item.id} onClick={() => onItemSelect(item)} />
        ))}
      </TableBody>
    </Table>
  );
}
```

**Media-Specific Implementation:**
```typescript
// Author/AuthorIndex.tsx
const AuthorIndex: React.FC = () => {
  const authors = useSelector(selectAllAuthors);
  
  const columns = [
    { name: 'name', label: 'Author Name' },
    { name: 'bookCount', label: 'Books' },
    { name: 'path', label: 'Path' }
  ];
  
  return <MediaIndex items={authors} columns={columns} />;
};
```

### 3. State Management Evolution

**Sonarr (Traditional Redux):**
```typescript
// Store structure
{
  series: {
    items: [],
    isPopulated: false,
    error: null
  },
  episodes: {
    items: {},
    isPopulated: false
  }
}
```

**Lidarr/Readarr (Redux Toolkit):**
```typescript
// Using Redux Toolkit slices
const authorsSlice = createSlice({
  name: 'authors',
  initialState: {
    items: [],
    status: 'idle',
    error: null
  },
  reducers: {
    authorAdded: (state, action) => {
      state.items.push(action.payload);
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAuthors.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchAuthors.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = action.payload;
      });
  }
});
```

### 4. Common UI Components

**Shared Component Library:**
```
Components/
├── Form/
│   ├── TextInput.tsx
│   ├── Select.tsx
│   └── FormGroup.tsx
├── Table/
│   ├── Table.tsx
│   ├── TableRow.tsx
│   └── VirtualTable.tsx
├── Modal/
│   ├── Modal.tsx
│   ├── ModalContent.tsx
│   └── ModalFooter.tsx
├── Page/
│   ├── PageContent.tsx
│   └── PageToolbar.tsx
└── Common/
    ├── LoadingIndicator.tsx
    ├── Icon.tsx
    └── Label.tsx
```

### 5. Media-Specific UI Adaptations

#### Series/Season/Episode View (Sonarr)
```typescript
<SeriesDetails>
  <SeriesHeader />
  <SeasonTabs>
    {seasons.map(season => (
      <SeasonTab key={season.id}>
        <EpisodeList episodes={season.episodes} />
      </SeasonTab>
    ))}
  </SeasonTabs>
</SeriesDetails>
```

#### Artist/Album/Track View (Lidarr)
```typescript
<ArtistDetails>
  <ArtistHeader />
  <AlbumGrid>
    {albums.map(album => (
      <AlbumCard key={album.id}>
        <AlbumCover />
        <TrackList tracks={album.tracks} />
      </AlbumCard>
    ))}
  </AlbumGrid>
</ArtistDetails>
```

#### Author/Book/Edition View (Readarr)
```typescript
<AuthorDetails>
  <AuthorHeader />
  <BookShelf>
    {books.map(book => (
      <BookCard key={book.id}>
        <BookCover />
        <EditionSelector editions={book.editions} />
      </BookCard>
    ))}
  </BookShelf>
  <SeriesConnections series={author.series} />
</AuthorDetails>
```

### 6. Search Interface Evolution

**Sonarr Search:**
```typescript
interface SeriesSearchResult {
  tvdbId: number;
  title: string;
  year: number;
  network: string;
  status: string;
}
```

**Lidarr Search:**
```typescript
interface ArtistSearchResult {
  foreignArtistId: string;
  artistName: string;
  disambiguation: string;
  artistType: string;
}
```

**Readarr Search:**
```typescript
interface AuthorSearchResult {
  foreignAuthorId: string;
  authorName: string;
  ratings: {
    votes: number;
    value: number;
  };
  books: BookSummary[];
}
```

### 7. Quality Profile UI Pattern

**Evolution of Quality Selection:**

```typescript
// Sonarr - Simple dropdown
<Select
  name="qualityProfileId"
  values={qualityProfiles}
  onChange={onQualityProfileChange}
/>

// Lidarr - Added metadata profile
<>
  <Select
    name="qualityProfileId"
    values={qualityProfiles}
    onChange={onQualityProfileChange}
  />
  <Select
    name="metadataProfileId"
    values={metadataProfiles}
    onChange={onMetadataProfileChange}
  />
</>

// Readarr - Enhanced with format preferences
<QualityProfileSelector
  qualityProfile={qualityProfile}
  metadataProfile={metadataProfile}
  formatItems={formatItems}
  onChange={onProfileChange}
/>
```

### 8. Calendar View Adaptations

**Media-Specific Calendar Items:**

```typescript
// Sonarr
interface CalendarEpisode {
  seriesTitle: string;
  seasonNumber: number;
  episodeNumber: number;
  airDateUtc: string;
  hasFile: boolean;
}

// Lidarr
interface CalendarAlbum {
  artistName: string;
  albumTitle: string;
  releaseDate: string;
  albumType: string;
  monitored: boolean;
}

// Readarr
interface CalendarBook {
  authorName: string;
  bookTitle: string;
  releaseDate: string;
  seriesTitle?: string;
  seriesPosition?: string;
}
```

### 9. Responsive Design Patterns

**Mobile Adaptations:**
```scss
// Media card responsive grid
.mediaGrid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
  
  @media (max-width: 768px) {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 10px;
  }
}

// Table to card transformation on mobile
@media (max-width: 768px) {
  .mediaTable {
    display: none;
  }
  
  .mediaCards {
    display: block;
  }
}
```

### 10. Internationalization Pattern

**i18n Implementation:**
```typescript
// Shared translation structure
const translations = {
  en: {
    common: {
      add: 'Add',
      delete: 'Delete',
      search: 'Search'
    },
    media: {
      // Sonarr
      series: 'Series',
      episode: 'Episode',
      // Lidarr
      artist: 'Artist',
      album: 'Album',
      // Readarr
      author: 'Author',
      book: 'Book'
    }
  }
};
```

## UI Migration Best Practices

### 1. Component Abstraction
- Create generic base components
- Use composition over inheritance
- Implement media-specific wrappers

### 2. State Management
- Normalize state shape early
- Use consistent action patterns
- Implement optimistic updates

### 3. Performance Optimization
- Virtualize long lists
- Implement lazy loading
- Use React.memo strategically

### 4. Accessibility
- Maintain ARIA labels
- Ensure keyboard navigation
- Test with screen readers

### 5. Testing Strategy
```typescript
// Shared test utilities
export function renderWithProviders(
  ui: React.ReactElement,
  {
    preloadedState = {},
    store = configureStore({ reducer, preloadedState }),
    ...renderOptions
  } = {}
) {
  function Wrapper({ children }: { children: React.ReactNode }) {
    return <Provider store={store}>{children}</Provider>;
  }
  return { store, ...render(ui, { wrapper: Wrapper, ...renderOptions }) };
}
```

## Common UI Challenges and Solutions

### Challenge 1: Terminology Consistency
**Solution**: Centralized terminology mapping
```typescript
const terminology = {
  primaryEntity: {
    sonarr: 'Series',
    lidarr: 'Artist',
    readarr: 'Author'
  },
  collection: {
    sonarr: 'Season',
    lidarr: 'Album',
    readarr: 'Book'
  }
};
```

### Challenge 2: Icon Adaptation
**Solution**: Media-specific icon sets
```typescript
const mediaIcons = {
  sonarr: {
    media: 'television',
    calendar: 'calendar-alt',
    missing: 'exclamation-triangle'
  },
  lidarr: {
    media: 'music',
    calendar: 'calendar-music',
    missing: 'compact-disc'
  },
  readarr: {
    media: 'book',
    calendar: 'calendar-book',
    missing: 'book-dead'
  }
};
```

### Challenge 3: Complex Relationships
**Solution**: Specialized relationship components
```typescript
// Readarr's SeriesConnection component
const SeriesConnection: React.FC<{ series: Series[] }> = ({ series }) => {
  return (
    <div className={styles.seriesConnection}>
      <h3>Part of Series</h3>
      {series.map(s => (
        <SeriesLink key={s.id} series={s} />
      ))}
    </div>
  );
};
```

## Future UI Considerations

### 1. Component Library
Consider extracting shared components into a separate package:
- `@servarr/ui-components`
- Shared across all *arr projects
- Consistent design system

### 2. Theme System
Implement a robust theming system:
```typescript
interface Theme {
  colors: {
    primary: string;
    secondary: string;
    background: string;
  };
  mediaSpecific: {
    accent: string;
    icon: string;
  };
}
```

### 3. Plugin Architecture
Enable UI extensions:
```typescript
interface UIPlugin {
  id: string;
  name: string;
  components: {
    settings?: React.ComponentType;
    mediaDetails?: React.ComponentType;
  };
}
```

## Conclusion

The UI migration patterns show:

1. **Consistent Architecture**: Shared component library and patterns
2. **Media-Specific Adaptations**: Tailored views for each media type
3. **Progressive Enhancement**: Each project improves on UI/UX
4. **Maintainable Structure**: Clear separation of concerns
5. **Performance Focus**: Optimizations for large libraries

These patterns enable rapid development of new media-specific UIs while maintaining consistency across the *arr ecosystem.