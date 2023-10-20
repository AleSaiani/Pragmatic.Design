module.exports = {
    '**/*.cs': (filenames) => {
        const relativePaths = filenames.map((filename) => filename.split('/src/')[1]);
        return `dotnet csharpier ${relativePaths.join(' ')}`;
    }
}
