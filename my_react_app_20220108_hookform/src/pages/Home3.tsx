import React from 'react';
import { useForm, SubmitHandler } from "react-hook-form";
import {List, ListItem} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

interface IFormInput {
  firstName: string;
  lastName: string;
  age: number;
}

const Home3 = () => {
  const { register, handleSubmit } = useForm<IFormInput>();
  const onSubmit: SubmitHandler<IFormInput> = (data) => {
    console.log(data);
  }
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              <label>First Name</label>
              <input {...register("firstName", { required: true, maxLength: 20 })} />
            </ListItem><ListItem>
              <label>Last Name</label>
              <input {...register("lastName", { pattern: /^[A-Za-z]+$/i })} />
            </ListItem><ListItem>
              <label>Age</label>
              <input type="number" {...register("age", { min: 18, max: 99 })} />
            </ListItem><ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home3;