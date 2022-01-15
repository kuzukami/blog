import React from 'react';
import { useForm, SubmitHandler } from "react-hook-form";
import {List, ListItem} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

interface IFormInputs {
  firstName: string
  lastName: string
}

const Home8 = () => {
  const { register, formState: { errors }, handleSubmit } = useForm<IFormInputs>();
  const onSubmit: SubmitHandler<IFormInputs> = (data) => {
    console.log(data);
  }
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              <label>First Name</label>
              <input {...register("firstName", { required: true })} />
              {errors.firstName && "First name is required"}
            </ListItem><ListItem>
              <label>Last Name</label>
              <input {...register("lastName", { required: true })} />
              {errors.lastName && "Last name is required"}
            </ListItem><ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home8;