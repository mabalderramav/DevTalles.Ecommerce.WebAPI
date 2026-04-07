using DevTalles.Ecommerce.WebAPI.Models;

namespace DevTalles.Ecommerce.WebAPI.Repository.IRepository
{
    /// <summary>
    /// Provides an abstraction for managing categories in the e-commerce system.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Retrieves a collection of all categories available in the e-commerce system.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Category"/> objects representing the available categories.
        /// </returns>
        ICollection<Category> GetCategories();
        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="categoryId">The unique identifier of the category.</param>
        /// <returns>
        /// A <see cref="Category"/> object representing the requested category, or null if not found.
        /// </returns>
        Category? GetCategory(int categoryId);
        /// <summary>
        /// Determines whether a category with the specified unique identifier exists in the e-commerce system.
        /// </summary>
        /// <param name="id">The unique identifier of the category to check for existence.</param>
        /// <returns>
        /// <c>true</c> if a category with the specified identifier exists; otherwise, <c>false</c>.
        /// </returns>
        bool CategoryExists(int id);
        /// <summary>
        /// Determines whether a category with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the category to check for existence. Cannot be null or empty.</param>
        /// <returns>true if a category with the specified name exists; otherwise, false.</returns>
        bool CategoryExists(string name);
        /// <summary>
        /// Creates a new category in the e-commerce system.
        /// </summary>
        /// <param name="category">The <see cref="Category"/> object representing the category to create.</param>
        /// <returns>true if the category was successfully created; otherwise, false.</returns>
        bool CreateCategory(Category category);
        /// <summary>
        /// Updates the specified category with new values.
        /// </summary>
        /// <param name="category">The category object containing updated values. Must not be null. The object's identifier is used to locate
        /// the existing category to update.</param>
        /// <returns>true if the category was successfully updated; otherwise, false.</returns>
        bool UpdateCategory(Category category);
        /// <summary>
        /// Deletes the specified category from the data store.
        /// </summary>
        /// <param name="category">The category to delete. Cannot be null.</param>
        /// <returns>true if the category was successfully deleted; otherwise, false.</returns>
        bool DeleteCategory(Category category);
        /// <summary>
        /// Attempts to persist the current state or data to the underlying storage.
        /// </summary>
        /// <returns>true if the save operation succeeds; otherwise, false.</returns>
        bool Save();
    }
}
