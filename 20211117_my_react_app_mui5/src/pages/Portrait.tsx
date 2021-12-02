import * as React from 'react';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Typography from '@mui/material/Typography';
import { CardActionArea } from '@mui/material';

import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';
import noa_xmas_icon from '../noa_xmas_icon.png';

const Portrait = () => {
  return (
    <ResponsiveDrawer>
      <div>
        <Card sx={{ maxWidth: 256 }}>
          <CardActionArea>
            <CardMedia
              component="img"
              height="256"
              image={noa_xmas_icon}
              alt="noa_xmas_icon"
            />
            <CardContent>
              <Typography gutterBottom variant="h5" component="div">
                noa
              </Typography>
              <Typography variant="body2" color="text.secondary">
              </Typography>
            </CardContent>
          </CardActionArea>
        </Card>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Portrait;