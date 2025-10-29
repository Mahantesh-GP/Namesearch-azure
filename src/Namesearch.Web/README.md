# Namesearch Web UI

## Overview
Modern Blazor Server UI for the Name Search application with reusable components and clean design.

## Components

### SearchBar
Reusable search input component with loading state.

**Props:**
- `Value` (string): The current search query
- `ValueChanged` (EventCallback<string>): Event fired when the value changes
- `OnSearchClicked` (EventCallback): Event fired when search is triggered
- `Placeholder` (string): Placeholder text
- `IsSearching` (bool): Shows loading spinner when true

**Usage:**
```razor
<SearchBar 
    Value="@searchQuery"
    ValueChanged="@(value => searchQuery = value)"
    OnSearchClicked="@PerformSearch"
    IsSearching="@isSearching"
    Placeholder="Enter name to search..." />
```

### FilterDropdown<TValue>
Generic dropdown component for filters.

**Props:**
- `Label` (string): Label text for the dropdown
- `Placeholder` (string): Placeholder for empty selection
- `Value` (TValue): Current selected value
- `ValueChanged` (EventCallback<TValue>): Event fired when selection changes
- `Options` (List<FilterOption<TValue>>): List of options to display

**Usage:**
```razor
<FilterDropdown 
    TValue="string"
    Label="County"
    Placeholder="All Counties"
    Value="@selectedCounty"
    ValueChanged="@(value => selectedCounty = value)"
    Options="@countyOptions" />
```

### SearchResults
Component to display search results with pagination.

**Props:**
- `Results` (List<SearchResult>): List of search results
- `TotalCount` (int): Total number of results
- `Page` (int): Current page number
- `PageSize` (int): Number of results per page
- `IsLoading` (bool): Shows loading state
- `HasSearched` (bool): Whether a search has been performed
- `OnPageChanged` (EventCallback<int>): Event fired when page changes

**Usage:**
```razor
<SearchResults 
    Results="@searchResponse.Results"
    TotalCount="@searchResponse.TotalCount"
    Page="@currentPage"
    PageSize="@pageSize"
    IsLoading="@isSearching"
    HasSearched="@hasSearched"
    OnPageChanged="@HandlePageChanged" />
```

## Styling

The application uses a modern, responsive design with:
- CSS custom properties for easy theming
- Mobile-first responsive layout
- Smooth transitions and hover effects
- Accessible color contrast
- Loading states and animations

### Color Scheme
- Primary: #0078d4 (Microsoft blue)
- Secondary: #50e6ff (Accent blue)
- Background: #f5f5f5 (Light gray)
- Surface: #ffffff (White)
- Text Primary: #323130 (Dark gray)
- Text Secondary: #605e5c (Medium gray)

### Customization
To customize the theme, edit the CSS custom properties in `wwwroot/css/search.css`:

```css
:root {
    --primary-color: #0078d4;
    --primary-hover: #106ebe;
    --secondary-color: #50e6ff;
    /* ... other variables */
}
```

## Design Principles

### 1. Reusability
All components are designed to be reusable across the application with clear prop interfaces.

### 2. Separation of Concerns
- **Components**: UI logic and presentation
- **Services**: API communication
- **Models**: Data structures

### 3. Responsiveness
All components adapt to different screen sizes with mobile-first CSS.

### 4. Accessibility
- Semantic HTML
- Proper ARIA labels
- Keyboard navigation support
- High contrast colors

### 5. Performance
- Server-side rendering with Blazor Server
- Optimized re-rendering
- Lazy loading where appropriate

## API Integration

The UI communicates with the backend API through the `ISearchApiClient` service:

```csharp
public interface ISearchApiClient
{
    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
}
```

The service is registered in `Program.cs` with an HttpClient configured to call the API.

## Development

### Adding New Components

1. Create a new `.razor` file in `Components/Shared/`
2. Define parameters using `[Parameter]` attribute
3. Add to `_Imports.razor` if needed
4. Create corresponding CSS in `wwwroot/css/` if needed

### Adding New Pages

1. Create a new `.razor` file in `Components/Pages/`
2. Add `@page "/route"` directive
3. Add `@rendermode InteractiveServer` for interactivity
4. Inject required services with `@inject`

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Edge (latest)
- Safari (latest)

## Future Enhancements

- [ ] Advanced search filters
- [ ] Export search results
- [ ] Save search queries
- [ ] Dark mode support
- [ ] Internationalization (i18n)
- [ ] Progressive Web App (PWA) support
