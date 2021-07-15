using System.Net;
using System;
using System.Web.Http;
using System.Web.Http.Description;
using Weavy.Core;
using Weavy.Core.Models;
using Weavy.Core.Services;
using Weavy.Web.Api.Controllers;
using Weavy.Web.Api.Models;
using Weavy.Web.Api.Streamers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Weavy.Core.Collections;
using NLog;

namespace Weavy.Areas.Api.Controllers
{

    /// <summary>
    /// The Roles API has methods for manipulating roles. 
    /// 
    /// 
    /// </summary>

    [RoutePrefix("api")]
    public class RoleController : WeavyApiController
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserts a new <see cref="Role"/>.
        /// </summary>
        /// <param name="model">The properties of the <see cref="Role"/> to insert.</param>
        /// <example> 
        /// POST /api/spaces
        ///
        /// {
        ///   "name": "A role bane"
        /// }
        /// </example>
        /// <returns>The inserted role.</returns>
        /// <example>
        /// 
        /// {
        ///    "id": 3,
        ///    "type": "role",
        ///    "name": "test role",
        ///    "is_member": false,
        ///    "created_at": "2020-10-14T01:59:16.1766667",
        ///    "created_by": {
        ///        "id": 2,
        ///        "type": "user",
        ///        "username": "testtt",
        ///        "name": "clientid",
        ///        "url": "/people/2",
        ///        "thumb_url": "/people/2/avatar-{options}.svg?v=c5a44f4"
        ///    },
        ///    "icon": {
        ///        "name": "account-multiple",
        ///        "color": "light-green"
        ///    },
        ///    "kind": "role",
        ///    "url": "/manage/roles/3"
        /// }
        /// </example>
        [HttpPost]
        [ResponseType(typeof(Role))]
        [Route("roles")]
        public IHttpActionResult Insert(Role model)
        {
            _log.Warn($"model = {model}");
            var role = RoleService.Insert(model);
            return Created($"/api/roles/{role.Id}", RoleService.Get(role.Id));
        }

        /// <summary>
        /// Get the role with the specified id.
        /// </summary>
        /// <param name="id">The Role id.</param>
        /// <returns>Returns the Role.</returns>
        [HttpGet]
        [ResponseType(typeof(Role))]
        [Route("roles/{id:int}")]
        public IHttpActionResult GetRole(int id)
        {

            var role = RoleService.Get(id);

            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }

            return Ok(role);
        }

        /// <summary>
        /// Updates the role with the specified id.
        /// </summary>
        /// <param name="id">The Role id.</param>
        /// <param name="model">The info to update for role model</param>
        /// <returns>Returns the updated Role.</returns>
        [HttpPatch]
        [ResponseType(typeof(Role))]
        [Route("roles/{id:int}")]
        public IHttpActionResult UpdateRole(int id, Delta<Role> model)
        {

            var role = RoleService.Get(id);

            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }
            model.Patch(role);
            RoleService.Update(role);
            return Ok(RoleService.Get(role.Id));
        }

        /// <summary>
        /// Trash a role.
        /// </summary>
        /// <param name="id">Id of the role to trash.</param>
        /// <returns>The trashed role.</returns>
        [HttpPost]
        [ResponseType(typeof(Role))]
        [Route("roles/{id:int}/trash")]
        public IHttpActionResult Trash(int id)
        {
            var role = RoleService.Get(id, trashed: true);
            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }
            role = RoleService.Trash(id);
            return Ok(role);
        }

        /// <summary>
        /// Restores a trashed role.
        /// </summary>
        /// <param name="id">Id of role to restore.</param>
        /// <returns>The restored role.</returns>
        [HttpPost]
        [ResponseType(typeof(Role))]
        [Route("roles/{id:int}/restore")]
        public IHttpActionResult Restore(int id)
        {
            var role = RoleService.Get(id, trashed: true);
            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }
            role = RoleService.Restore(id);
            return Ok(role);
        }

        /// <summary>
        /// Delete a role.
        /// </summary>
        /// <param name="id">Id of the role to delete.</param>
        /// <returns>The deleted role.</returns>
        [HttpDelete]
        [ResponseType(typeof(Role))]
        [Route("roles/{id:int}/delete")]
        public IHttpActionResult Delete(int id)
        {
            var role = RoleService.Get(id);
            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }
            role = RoleService.Delete(id);
            return Ok(role);
        }


        /// <summary>
        /// Get Members of a specified role.
        /// </summary>
        /// <param name="id">Id of the role to get members of.</param>
        /// <returns>The roles members.</returns>
        [HttpGet]
        [ResponseType(typeof(Role))]
        [Route("roles/{id:int}/members")]
        public IHttpActionResult GetMembers(int id)
        {
            var role = RoleService.Get(id);
            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }
            var members = RoleService.GetMembers(id);
            return Ok(members);
        }


        /// <summary>
        /// Add a Member to a specified role.
        /// </summary>
        /// <param name="id">Id of the role to get members of.</param>
        /// <param name="userId">Id of the role to get members of.</param>
        /// <returns>The User that was added.</returns>
        [HttpPost]
        [ResponseType(typeof(User))]
        [Route("roles/{id:int}/members/add/{userId:int}")]
        public IHttpActionResult AddMember(int id, int userId)
        {
            var role = RoleService.Get(id);
            if (role == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "Role with id " + id + " not found");
            }
            var user = UserService.Get(userId);
            if (user == null)
            {
                ThrowResponseException(HttpStatusCode.NotFound, "User with id " + id + " not found");
            }
            var newMember = RoleService.AddMember(id, userId);
            return Ok(newMember);
        }



        /*        /// <summary>
                /// Get all roles
                /// </summary>
                /// <param name="query"></param>
                /// <returns>Returns the Role.</returns>
                [HttpGet]
                [ResponseType(typeof(ScrollableList<Role>))]
                [Route("roles")]
                public IHttpActionResult GetRoles(RoleQuery query)
                {
                  //  query.AppId = null;
                  //  var roles = RoleService.GetRoles();

                    return Ok(roles);
                }
        */




    }
}
