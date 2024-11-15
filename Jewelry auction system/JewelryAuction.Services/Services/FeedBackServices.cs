using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Repositories;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class FeedBackServices : IFeedBackServices
    {
        private readonly IFeedBackRepository _feedBackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuctionsRepository _auctionsRepository;

        public FeedBackServices(IFeedBackRepository feedBackRepository, IUserRepository userRepository, IAuctionsRepository auctionsRepository)
        {
            _feedBackRepository = feedBackRepository;
            _userRepository = userRepository;
            _auctionsRepository = auctionsRepository;
        }

        #region add feedback
        public async Task AddFeedback(string userId, string auctionId, FeedBackContentDTO feedBackContentDTO)
        {
            var user = await _userRepository.GetUserById(userId);
            var auction = await _auctionsRepository.GetAuctionsbyID(auctionId);
            if (user == null)
            {
                throw new KeyNotFoundException("Cannot find this user Id");
            }
            else
            if (auction == null)
            {
                throw new KeyNotFoundException("Cannot find this auction");
            }
            else
            {
                if (auction.Status != AuctionStatus.Done && auction.Status != AuctionStatus.None)
                {
                    throw new KeyNotFoundException("Auction status must be Done or None to add feedback.");
                }
                else
                if (string.IsNullOrWhiteSpace(feedBackContentDTO.Content))
                {
                    throw new KeyNotFoundException("Content cannot be null.");
                }
                else
                if (await _feedBackRepository.FeedbackExistsAsync(userId, auctionId))
                {
                    throw new KeyNotFoundException("User has already provided feedback for this auction.");
                }
                else
                {
                    var newFeedback = new FeedBacks
                    {
                        UserId = userId,
                        AuctionId = auctionId,
                        UpdatedDate = DateTime.UtcNow,
                        Status = FeedBackStatus.Active,
                        Content = feedBackContentDTO.Content,
                    };

                    await _feedBackRepository.AddFeedback(newFeedback);
                }
            }
        }
        #endregion

        #region edit feedback
        public async Task EditFeedBack(string userId, string auctionId, FeedBackContentDTO feedBackContentDTO)
        {
            var user = await _userRepository.GetUserById(userId);
            var auction = await _auctionsRepository.GetAuctionsbyID(auctionId);
            if (user == null)
            {
                throw new KeyNotFoundException("Cannot find this user Id");
            }
            else
            if (auction == null)
            {
                throw new KeyNotFoundException("Cannot find this auction");
            }
            else
            {
                if (auction.Status != AuctionStatus.Done)
                {
                    throw new KeyNotFoundException("Auction status must be Done to edit feedback.");
                }
                else
                if (string.IsNullOrWhiteSpace(feedBackContentDTO.Content))
                {
                    throw new KeyNotFoundException("Content cannot be null.");
                }
                else
                if (await _feedBackRepository.FeedbackExistsAsync(userId, auctionId))
                {

                    var fb = await _feedBackRepository.GetFeedbackByUserIdAndAuctionId(userId, auctionId);
                    if (fb.Status != FeedBackStatus.Edited)
                    {
                        fb.UpdatedDate = DateTime.Now;
                        fb.Status = FeedBackStatus.Edited;
                        fb.Content = feedBackContentDTO.Content;

                        await _feedBackRepository.UpdateFeedback(fb);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Feedback cannot be edit twice.");
                    }

                }
                else
                {
                    throw new KeyNotFoundException("Cannot find user feedback on this auction.");
                }
            }
        }
        #endregion

        #region Disable feedback
        public async Task DisableFeedBack(string userId, string auctionId)
        {
            var user = await _userRepository.GetUserById(userId);
            var auction = await _auctionsRepository.GetAuctionsbyID(auctionId);
            if (user == null)
            {
                throw new KeyNotFoundException("Cannot find this user Id");
            }
            else
            if (auction == null)
            {
                throw new KeyNotFoundException("Cannot find this auction");
            }
            else
            if (await _feedBackRepository.FeedbackExistsAsync(userId, auctionId))
            {
                var fb = await _feedBackRepository.GetFeedbackByUserIdAndAuctionId(userId, auctionId);
                if (fb.Status != FeedBackStatus.Disable)
                {
                    fb.UpdatedDate = DateTime.Now;
                    fb.Status = FeedBackStatus.Disable;

                    await _feedBackRepository.UpdateFeedback(fb);
                }
                else
                {
                    throw new KeyNotFoundException("Can not disable feedback that already been disabled.");
                }
            }
            else
            {
                throw new KeyNotFoundException("Cannot find user feedback on this auction.");
            }

        }
        #endregion

        #region View feedback

        public async Task<FeedBackDTO> GetFeedbackById(string feedbackId)
        {
            var fb = await _feedBackRepository.GetFeedbackById(feedbackId);
            if (fb == null)
            {
                throw new KeyNotFoundException("This feedback does not exist.");
            }
            var user = await _userRepository.GetUserById(fb.UserId);
            return new FeedBackDTO
            {
                ID = fb.ID,
                CreatedDate = fb.CreatedDate,
                UpdatedDate = fb.UpdatedDate,
                UserId = fb.UserId,
                AuctionId = fb.AuctionId,
                Content = fb.Content,
                Status = fb.Status.ToString(),
                FullName = user.FullName,
            };
        }

        public async Task<IEnumerable<FeedBackDTO>> GetAllFeedbacksByAuctionId(string auctionId)
        {
            var au = await _auctionsRepository.GetSimpleAuction(auctionId);
            if (au == null)
            {
                throw new KeyNotFoundException("Cannot find this auction.");
            }
            else
            {
                var feedbacks = await _feedBackRepository.GetAllFeedbacksByAuctionId(auctionId, FeedBackStatus.Active);
                if (!feedbacks.Any())
                {
                    throw new KeyNotFoundException("Cannot find any feedback of this auction.");
                }
                else
                {
                    var fbDTOs = new List<FeedBackDTO>();
                    foreach (var feedback in feedbacks)
                    {
                        var user = await _userRepository.GetUserById(feedback.UserId);

                        fbDTOs.Add(new FeedBackDTO
                        {
                            ID = feedback.ID,
                            CreatedDate = feedback.CreatedDate,
                            UpdatedDate = feedback.UpdatedDate,
                            UserId = feedback.UserId,
                            AuctionId = feedback.AuctionId,
                            Content = feedback.Content,
                            Status = feedback.Status.ToString(),
                            FullName = user.FullName,

                        });
                    }
                    return fbDTOs;
                    /*return feedbacks.Select(f => new FeedBackDTO
                    {
                        ID = f.ID,
                        CreatedDate = f.CreatedDate,
                        UpdatedDate = f.UpdatedDate,
                        UserId = f.UserId,
                        AuctionId = f.AuctionId,
                        Content = f.Content,
                        Status = f.Status.ToString()
                        FullName = 
                    });*/
                }
            }
        }
        #endregion
    }
}
