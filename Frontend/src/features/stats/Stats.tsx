import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { getUserIdCookie } from '../../app/helpers/helpers';

export const Stats = () => {
    const navigate = useNavigate();

    useEffect(() => {
        // Redirect to login if not authenticated
        if (!getUserIdCookie()) {
            navigate('/login');
        }
    });
    
    return (
        <div className="stats container">
            <div className="panel-container panel-lg">
                <h2 className="colored-header text-center">Statistics</h2>
                <div className="panel-body-container">
                    Total Time
                    TBD
                </div>
            </div>

        </div>
    );
};