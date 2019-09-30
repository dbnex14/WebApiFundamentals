using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Route()]
        public async Task<IHttpActionResult> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var results = await _repository.GetTalksByMonikerAsync(moniker, includeSpeakers);

                return Ok(_mapper.Map<IEnumerable<TalkModel>>(results));
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("{id:int}", Name = "GetTalk")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<TalkModel>(result));
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route()]
        public async Task<IHttpActionResult> Post(string moniker, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var camp = await _repository.GetCampAsync(moniker);
                    if (camp != null)
                    {
                        var talk = _mapper.Map<Talk>(model);
                        talk.Camp = camp;

                        // mapp speaker if necessary
                        if (model.Speaker != null)
                        {
                            var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                            if (speaker != null)
                            {
                                talk.Speaker = speaker;
                            }
                        }

                        _repository.AddTalk(talk);
                        if(await _repository.SaveChangesAsync())
                        {
                            return CreatedAtRoute("GetTalk"
                                , new { moniker = moniker, id = talk.TalkId }
                                , _mapper.Map<TalkModel>(talk));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId, true);
                    if (talk == null) return NotFound();

                    // we map TalkModel to Talk, dont override Speaker and Camp when updating Talk.  To prevent overriding
                    // we set Ignore for Speaker and Camp in CampMappingProfile when going from TalkModel to Talk
                    _mapper.Map(model, talk);

                    // Since we are ignoring Speaker when mapping above, we need to change speaker if needed.
                    // In effect, we said above ignore updating speaker, we will do it outself, so we do it here.
                    if (talk.Speaker.SpeakerId != model.Speaker.SpeakerId)
                    {
                        var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                        if (speaker != null) talk.Speaker = speaker;
                    }
                    if (await _repository.SaveChangesAsync())
                    {
                        return Ok(_mapper.Map<TalkModel>(talk));
                    }
                }
                else
                {

                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Delete(string moniker, int talkId)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId);
                if (talk == null) return NotFound();

                _repository.DeleteTalk(talk);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}