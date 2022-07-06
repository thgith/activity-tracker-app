import { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom'
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { getUser } from '../../User/userSlice';
import { ISession } from '../../sessions/ISession';
import { IActivity } from '../IActivity';
import { Loader } from '../../../app/views/Loader';
import { listActivities } from '../activityMethods';

const displayTags = (tags: string[]) => {
    let tagItems: any = [];
    tags.forEach(tag => {
        tagItems.push(<span className="tag" key={tag}>{tag}</span>);
    });
    return tagItems;
}

export const ActivitiesList = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const gotFullListBefore = useSelector((state: any) => state.activitiesData.gotFullListBefore);
    const { user: currentUser } = useSelector((state: any) => state.userData);
    const activities = useSelector((state: any) => state.activitiesData.activities);
    const [stringFilter, setStringFilter] = useState('');
    const activityIdToSessions = useSelector((state: any) => state.activitiesData.activityIdToSessions);

    useEffectSkipInitialRender(() => {
        const currUserId = getUserIdCookie();
        // Redirect to login if not authenticated
        if (!currUserId) {
            navigate('/login');
        } else if (!currentUser) {
            // Get user if we don't have them
            dispatch<any>(getUser(currUserId))
                .unwrap()
                .then(() => {
                })
                .catch(() => {
                });
        }
        // Only send request if we have a user and we haven't gotten any activities yet
        else if (!gotFullListBefore) {
            dispatch<any>(listActivities(currUserId))
                .unwrap()
                .then(() => {
                })
                .catch(() => {
                });
        }
    });

    const calculateActivityHours = (activityId: string) => {
        let totalSeconds = 0;
        let sessions = activityIdToSessions ? activityIdToSessions[activityId] : [];
        sessions.map((x: ISession) => {
            totalSeconds += x.durationSeconds
        });

        return Math.round(totalSeconds / 3600 * 10) / 10;
    }

    const getTotalHours = () => {
        let totalSeconds = 0;
        if (activityIdToSessions) {
            Object.keys(activityIdToSessions).forEach((k: string) => {
                let sessions = activityIdToSessions[k];
                sessions.map((x: ISession) => {
                    totalSeconds += x.durationSeconds
                })
            });
        }
        return Math.round(totalSeconds / 3600 * 10) / 10;
    }

    const updateStringFilter = (e: any) => {
        console.log(e.target.value);
        setStringFilter(e.target.value.toLowerCase());
    }

    const tagsContainFilter = (tags: string[] | null, filterStr: string) => {
        if (!tags) {
            return false;
        }

        let tagsContainFilter = false;
        tags.forEach((tag) => {
            let val = tag.toLowerCase().includes(filterStr);
            if (tag.toLowerCase().includes(filterStr)) {
                tagsContainFilter = true;
                return;
            }            
        })
        return tagsContainFilter;
    }

    const isActivityIncludedInFilter = (activity: IActivity) => {
        return (activity.name?.toLowerCase().includes(stringFilter)) ||
            (activity.description?.toLowerCase().includes(stringFilter)) ||
            tagsContainFilter(activity.tags, stringFilter);
    }

    if (!gotFullListBefore) {
        return <Loader />
    }

    const noActivities = (
        <div className="no-activities text-center">
            <div className="fa fa-sticky-note-o fa-5x"></div>
            <h5 className="no-activities-body">No Activities. <Link to="/activities/new">Create one now!</Link></h5>
        </div>
    )

    const renderedActivities = activities.map((activity: IActivity) => (
        isActivityIncludedInFilter(activity) ?
        <div className="card-container activity-item col-md-4 col-sm-6 mt-2" key={activity.id}>
            <div className="card-item">
                <Link to={`/activities/${activity.id}`} style={{ textDecoration: 'none' }}>
                    <div className="activity-header card-header" style={{ backgroundColor: activity.colorHex as string }}>
                        <h3 className="text-center">{activity.name}</h3>
                    </div>
                    <div className="activity-body card-body">
                        <div className="activity-hours">
                            {calculateActivityHours(activity.id as string)} hours over {activityIdToSessions ? activityIdToSessions[activity.id as string].length : 0} sessions
                        </div>
                        <div className="tags">
                            {displayTags(activity.tags as string[])}
                        </div>
                        <div className="activity-description">{activity.description}</div>
                    </div>
                </Link>
            </div>
        </div>
    : null));

    return (
        <div className="activities-list container">
            <h1 className="text-center">Activities</h1>
            <p className="text-center">
                <b>
                    <span className="fa fa-clock-o"></span>
                    <span>Total Time: {getTotalHours()} hours</span>
                </b>
            </p>
            <div className="row">
                <div className="col-1">
                    <button className="btn btn-secondary">
                        <span className="fa fa-list"></span>
                        <span className="d-none d-md-inline-block">List</span>
                    </button>
                </div>
                <div className="col-9">
                    <div className="input-group rounded">
                        <input
                            type="search"
                            className="form-control search-bar rounded"
                            placeholder="Filter by name, description, or tags"
                            aria-label="Search"
                            aria-describedby="search-addon"
                            onKeyUp={updateStringFilter}
                            />
                        <span className="input-group-text border-0" id="search-addon">
                            <span className="fa fa-search"></span>
                        </span>
                    </div>
                </div>
                <div className="col-2">
                    <Link to="/activities/new">
                        <button className="save-activity-btn btn btn-primary">
                            <span className="fa fa-plus"></span>
                            <span>Add New Activity</span>
                        </button>
                    </Link>
                </div>
            </div>
            <div className="row">
                {
                    activities.length > 0 ?
                        renderedActivities :
                        noActivities
                }
            </div>
        </div>
    )
}