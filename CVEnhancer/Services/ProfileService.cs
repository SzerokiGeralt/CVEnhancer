using CVEnhancer.Data;
using CVEnhancer.Models;
using Microsoft.EntityFrameworkCore;

namespace CVEnhancer.Services;

public class ProfileService
{
    public async Task<User?> GetUserAsync(int userId)
    {
        using var db = new AppDbContext();
        return await db.Users
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateUserAsync(User updated)
    {
        using var db = new AppDbContext();

        var user = await db.Users
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(u => u.UserId == updated.UserId);

        if (user == null) throw new InvalidOperationException("User not found.");

        // mapuj pola (tylko to, co edytujesz w formularzu)
        user.FirstName = updated.FirstName;
        user.LastName = updated.LastName;
        user.Email = updated.Email;
        user.PhoneNumber = updated.PhoneNumber;
        user.LinkedInUrl = updated.LinkedInUrl;
        user.GitHubUrl = updated.GitHubUrl;
        user.PortfolioUrl = updated.PortfolioUrl;
        user.JobTitle = updated.JobTitle;
        user.ProfessionalSummary = updated.ProfessionalSummary;

        // jeśli edytujesz zdjęcie:
        if (updated.ProfilePicture?.Picture != null)
        {
            if (user.ProfilePicture == null)
                user.ProfilePicture = new ProfilePicture();

            user.ProfilePicture.Picture = updated.ProfilePicture.Picture;
        }

        await db.SaveChangesAsync();
    }
}
