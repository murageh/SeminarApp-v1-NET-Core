using SeminarIntegration.DTOs;
using SeminarIntegration.Models;
using SeminarIntegration.Services;

namespace SeminarIntegration.Interfaces;

public interface ISeminar
{
    /// <summary>
    ///     This creates a new seminar using provided values.
    /// </summary>
    /// <param name="Name">The name of the seminar.</param>
    /// <param name="SeminarDuration">The duration of the seminar.</param>
    /// <param name="SeminarPrice">The price of the seminar.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created seminar.</returns>
    Task<AppResponse<Services.Seminar>.BaseResponse> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice);

    // /// <summary>
    // /// This creates a new seminar using provided values.
    // /// </summary>
    // /// <param name="Name">The name of the seminar.</param>
    // /// <param name="SeminarDuration">The duration of the seminar.</param>
    // /// <param name="SeminarPrice">The price of the seminar.</param>
    // /// <param name="MinParticipants">The minimum number of participants.</param>
    // /// <param name="MaxParticipants">The maximum number of participants.</param>
    // /// <param name="Blocked">Indicates if the seminar is blocked.</param>
    // /// <param name="GenProdPostingGroup">The general product posting group.</param>
    // /// <param name="VATProdPostingGroup">The VAT product posting group.</param>
    // /// <returns>A task that represents the asynchronous operation. The task result contains the created seminar.</returns>
    // Task<AppResponse<Seminar>.BaseResponse> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice, int MinParticipants, int MaxParticipants, bool Blocked, string GenProdPostingGroup, string VATProdPostingGroup);

    // /// <summary>
    // /// This creates a new seminar using the provided seminar data.
    // /// </summary>
    // /// <param name="seminar">The seminar data.</param>
    // /// <returns>A task that represents the asynchronous operation. The task result contains the created seminar.</returns>
    // Task<AppResponse<Seminar>.BaseResponse> CreateSeminar(Seminar seminar);

    /// <summary>
    ///     Retrieves all seminars.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the seminars.</returns>
    Task<AppResponse<List<Services.Seminar>>.BaseResponse> GetSeminars();

    /// <summary>
    ///     Retrieves a seminar using the seminar number.
    /// </summary>
    /// <param name="seminarNo"></param>
    /// <returns></returns>
    Task<AppResponse<Services.Seminar>.BaseResponse> GetSeminar(string seminarNo);

    /// <summary>
    ///     Deletes a seminar using the seminar number.
    /// </summary>
    /// <param name="seminarNo">The seminar number.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted status and message.</returns>
    Task<AppResponse<object>.BaseResponse> DeleteSeminar(string seminarNo);

    /// <summary>
    ///     This updates the seminar using bound actions (SOAP).
    /// </summary>
    /// <param name="seminar"></param>
    /// <returns></returns>
    Task<AppResponse<object>.BaseResponse> UpdateSeminarSoap(Services.Seminar seminar);

    /// <summary>
    ///     This updates the seminar using unbound actions (ODATA-V4), as compared to the bound actions in the previous
    ///     `UpdateSeminarSoap` method.
    /// </summary>
    /// <param name="seminar"></param>
    /// <returns></returns>
    Task<AppResponse<Services.Seminar>.BaseResponse> UpdateSeminar(Services.Seminar seminar);

    /// <summary>
    ///     Retrieves all general product posting groups.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the general product posting groups.</returns>
    Task<AppResponse<List<GenProdPostingGroupDto>>.BaseResponse> GetGenProdPostingGroups();

    /// <summary>
    ///     Retrieves all VAT product posting groups.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the VAT product posting groups.</returns>
    Task<AppResponse<List<VATProdPostingGroupDto>>.BaseResponse> GetVATProdPostingGroups();

    /// <summary>
    ///     Retrieves all contacts for a specific company.
    /// </summary>
    /// <param name="companyName">The name of the company.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the contacts.</returns>
    Task<AppResponse<List<Contact>>.BaseResponse> GetContacts(string companyName);

    /// <summary>
    ///     Retrieves all available seminars.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the available seminars.</returns>
    Task<AppResponse<List<AvailableSeminar>>.BaseResponse> GetAvailableSeminars();

    /// <summary>
    ///     Retrieves a single available seminar using the seminar number.
    /// </summary>
    /// <param name="seminarNo">The seminar number.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the available seminar.</returns>
    Task<AppResponse<AvailableSeminar>.BaseResponse> GetAvailableSeminar(string seminarNo);

    /// <summary>
    ///     Adds a seminar registration.
    /// </summary>
    /// <param name="semNo">The seminar number.</param>
    /// <param name="companyNo">The company number.</param>
    /// <param name="participantContactNo">The contact number of the participant.</param>
    /// <param name="confirmation">Confirmation status.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the registration response.</returns>
    Task<AppResponse<SeminarRegistrationItem>.BaseResponse> CreateSeminarRegistration(string semNo, string companyNo, string participantContactNo, bool? confirmation);

    /// <summary>
    ///     Updates a seminar registration.
    /// </summary>
    /// <param name="semHeaderNo">The seminar number.</param>
    /// <param name="lineNo">The line number of the registration.</param>
    /// <param name="confirmed">Confirmation status.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the registration response.</returns>
    Task<AppResponse<SeminarRegistrationItem>.BaseResponse> UpdateSeminarRegistration(string semHeaderNo, int lineNo, bool confirmed);

    /// <summary>
    ///     Retrieves seminar registrations for a specific participant and seminar.
    /// </summary>
    /// <param name="participantContactNo">The contact number of the participant.</param>
    /// <param name="seminarNo">The seminar number.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the seminar registrations.</returns>
    Task<AppResponse<List<SeminarRegistrationRespItem>>.BaseResponse> GetSeminarRegistrations(string participantContactNo, string? seminarNo="");
}